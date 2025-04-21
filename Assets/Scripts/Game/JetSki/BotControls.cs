using System;
using UnityEngine;

public class BotControls : MonoBehaviour, IJetSkiControls
{
    [SerializeField] float _xRange = 5;
    [SerializeField] float _verticalCorrectionTime = 5;
    [SerializeField] float _verticalSmoothing = 0.1f;
    [SerializeField] float _horizontalSmoothing = 0.1f;
    [SerializeField] float _distanceOffset = 0; // some customizing for different bots
    [SerializeField] JetSkiController _player;
    [SerializeField] JetSkiController _bot;


    Vector2 _moveInput;
    float _xSmoothVelocity;
    float _ySmoothVelocity;


    [SerializeField, Header("Wandering")] UniformValue _wanderWaitTime = new (2, 1);
    [SerializeField] UniformValue _wanderDuration = new (1.5f, 0.75f);
    [SerializeField] UniformValue _wanderAmplitude = new (15, 7.5f);
    [SerializeField] float _wanderSmoothTime = 0.1f;

    float _targetWanderOffset;
    float _nextWanderTime = 0;
    bool _wanderStarted = false;
    float _currentWanderTime;
    float _currentWanderSmoothVelocity;
    float _currentWanderX;


    void Start()
    {
        _nextWanderTime = _wanderWaitTime.GetRandomValue();
    }

    public void Update()
    {
        float playerVelocity = _player.Velocity;
        // distance is used to determine the target speed
        float distance = _player.transform.position.z - transform.position.z;
        distance += playerVelocity / 2.0f; // try(hope) to be in front of the player
        distance += _distanceOffset;
        float targetSpeed = playerVelocity + distance / _verticalCorrectionTime;
        targetSpeed = Mathf.Clamp(targetSpeed, _bot.JetSkiStats.MinSpeed, _bot.JetSkiStats.MaxSpeed);
        float targetVerticalInput = MathExtensions.LinearMap(targetSpeed, 
                                    _bot.JetSkiStats.MinSpeed, _bot.JetSkiStats.MaxSpeed, -1f, 1f);
        _moveInput.y = Mathf.SmoothDamp(_moveInput.y, targetVerticalInput, ref _ySmoothVelocity, _verticalSmoothing);

        ProcessWandering();
        float targetXPos = transform.position.x + _currentWanderX;
        targetXPos = Mathf.Clamp(targetXPos, -_xRange, _xRange);
        float currentYawAngle = Vector3.SignedAngle(Consts.CorridorForward, transform.forward, Vector3.up);
        Vector3 targetHorPos = new(targetXPos, transform.position.y, transform.position.z + _bot.Velocity);
        float targetYawAngle = Quaternion.LookRotation(targetHorPos - transform.position).eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(currentYawAngle, targetYawAngle);
        float targetHorizontalInput = MathExtensions.LinearMap(angleDiff, 
                                      -_bot.JetSkiStats.MaxTurnAngle, _bot.JetSkiStats.MaxTurnAngle, -1f, 1f);
        _moveInput.x = Mathf.SmoothDamp(_moveInput.x, targetHorizontalInput, ref _xSmoothVelocity, _horizontalSmoothing);
    }

    void ProcessWandering()
    {
        if (Time.time > _nextWanderTime && !_wanderStarted)
        {
            _wanderStarted = true;
            _currentWanderTime = _wanderDuration.GetRandomValue();
            int wanderDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            _targetWanderOffset = _wanderAmplitude.GetRandomValue() * wanderDirection;
        }
        else if (_wanderStarted)
        {
            _currentWanderTime -= Time.deltaTime;
            if (_currentWanderTime < 0)
            {
                _wanderStarted = false;
                _nextWanderTime = Time.time + _wanderWaitTime.GetRandomValue();
            }
            else
            {
                _currentWanderX = Mathf.SmoothDamp(_currentWanderX, _targetWanderOffset, ref _currentWanderSmoothVelocity, _wanderSmoothTime);
            }
        }
        else
        {
            _currentWanderX = Mathf.SmoothDamp(_currentWanderX, 0, ref _currentWanderSmoothVelocity, _wanderSmoothTime);
        }
    }

    public float GetHorizontalInput()
    {
        return _moveInput.x;
    }

    public float GetVerticalInput()
    {
        return _moveInput.y;
    }

    void OnDrawGizmosSelected()
    {
        if (_player == null || _bot == null || _player.Rigidbody == null || _bot.Rigidbody == null)
            return;

        float playerVelocity = _player.Velocity;
        float distance = _player.transform.position.z - transform.position.z;
        distance += playerVelocity / 2.0f;
        distance += _distanceOffset;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * distance, 0.2f);

        float targetXPos = Mathf.Clamp(transform.position.x + _currentWanderX, -_xRange, _xRange);
        Vector3 targetHorPos = new(targetXPos, 1, transform.position.z + 5);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetHorPos, 0.3f);
    }
}
