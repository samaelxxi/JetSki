using System;
using UnityEngine;

public class BotControls : MonoBehaviour, IJetSkiControls
{
    [SerializeField] float _xRange = 5;
    [SerializeField] float _verticalCorrectionTime = 5;
    [SerializeField] float _verticalSmoothing = 0.1f;
    [SerializeField] float _horizontalSmoothing = 0.1f;
    [SerializeField] JetSkiController _player;
    [SerializeField] JetSkiController _bot;


    Vector2 _moveInput;
    float _xSmoothVelocity;
    float _ySmoothVelocity;


    public void Update()
    {
        // TODO FIX
        float playerVelocity = _player.Velocity.z;
        float distance = _player.transform.position.z - transform.position.z;
        distance += playerVelocity / 2.0f; // try to be in front of the player
        float targetSpeed = playerVelocity + distance / _verticalCorrectionTime;
        targetSpeed = Mathf.Clamp(targetSpeed, _bot.JetSkiStats.MinSpeed, _bot.JetSkiStats.MaxSpeed);
        float targetVerticalInput = MathExtensions.LinearMap(targetSpeed, _bot.JetSkiStats.MinSpeed, _bot.JetSkiStats.MaxSpeed, -1f, 1f);
        _moveInput.y = Mathf.SmoothDamp(_moveInput.y, targetVerticalInput, ref _ySmoothVelocity, _verticalSmoothing);

        // it's kinda eh but it works, could add horizontal target pos floating to make it less dull tho but no time
        float currentYawAngle = Vector3.SignedAngle(Consts.CorridorForward, transform.forward, Vector3.up);
        float targetXPos = Mathf.Clamp(transform.position.x, -_xRange, _xRange);
        Vector3 targetHorPos = new(targetXPos, 0, transform.position.z + _bot.Velocity.z);
        float targetYawAngle = Quaternion.LookRotation(targetHorPos - transform.position.SetY(0)).eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(currentYawAngle, targetYawAngle);
        float targetHorizontalInput = MathExtensions.LinearMap(angleDiff, -_bot.JetSkiStats.MaxTurnAngle, _bot.JetSkiStats.MaxTurnAngle, -1f, 1f);
        _moveInput.x = Mathf.SmoothDamp(_moveInput.x, targetHorizontalInput, ref _xSmoothVelocity, _horizontalSmoothing);
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

        float playerVelocity = _player.Velocity.z;
        float distance = _player.transform.position.z - transform.position.z;
        distance += playerVelocity / 2.0f; // try to be in front of the player
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * distance, 0.1f);


        float targetXPos = Mathf.Clamp(transform.position.x, -_xRange, _xRange);
        Vector3 targetHorPos = new(targetXPos, 1, transform.position.z + 2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetHorPos, 0.3f);
    }
}
