using UnityEngine;

public class JetSkiController : MonoBehaviour
{
    [SerializeField] JetSkiStats _jetSkiStats;
    [SerializeField] Transform _jetMesh;
    [SerializeField] BuoyancyObject _buoyancy;
    [SerializeField] PilotLeanRig _pilotLeanRig;


    public bool IsUnderWater => _buoyancy.IsUnderWater;
    public float Velocity => _currentSpeed;
    public JetSkiStats JetSkiStats => _jetSkiStats;
    public Rigidbody Rigidbody => _rigidbody;

    IJetSkiControls _controls;
    float _currentSpeed;
    float _currentRoll;
    Rigidbody _rigidbody;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxAngularVelocity = 10f;
        _controls = GetComponent<IJetSkiControls>();
        _pilotLeanRig.SetControls(_controls);
    }

    void Update()
    {
        ApplyTurnLean();
    }

    void FixedUpdate()
    {
        MoveForward();
        TurnJet();

        CorrectPitch();
        DumpAngularSpeed();
    }

    void DumpAngularSpeed()
    {
        Vector3 angVel = _rigidbody.angularVelocity;
        angVel = Vector3.ClampMagnitude(angVel, 5);
        angVel.x *= 0.5f;
        angVel.y *= 0.9f;
        angVel.z *= 0.5f;
        _rigidbody.angularVelocity = angVel;
    }

    void MoveForward()
    {
        if (!_buoyancy.IsUnderWater)
            return;

        float verticalInput = _controls.GetVerticalInput();
        float speedLerpFactor = (verticalInput + 1f) * 0.5f;  // Normalize input from [-1, 1] to [0, 1]
        float targetSpeed = Mathf.Lerp(_jetSkiStats.MinSpeed, _jetSkiStats.MaxSpeed, speedLerpFactor);

        if (Vector3.Dot(Consts.CorridorForward, transform.forward.SetY(0)) > 0)
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _jetSkiStats.Acceleration * Time.fixedDeltaTime);
        else
            _currentSpeed *= Mathf.MoveTowards(_currentSpeed, _jetSkiStats.MinSpeed, _jetSkiStats.Deceleration * Time.fixedDeltaTime);

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 desiredVelocity = flatForward * _currentSpeed;
         _rigidbody.AddForce(desiredVelocity, ForceMode.Acceleration);
    }

    void TurnJet()
    {
        float horizontalInput = _controls.GetHorizontalInput();

        if (ShouldApplyTurnInput(horizontalInput))
        {
            _rigidbody.AddRelativeTorque(Vector3.up * horizontalInput * _jetSkiStats.TurnTorque, ForceMode.Force);
        }
    }

    bool ShouldApplyTurnInput(float horizontalInput)
    {
        if (Mathf.Abs(horizontalInput) < 0.1f)
            return false;

        float currentYawAngle = Vector3.SignedAngle(Consts.CorridorForward, transform.forward, Vector3.up);

        if (horizontalInput > 0 && currentYawAngle >= _jetSkiStats.MaxTurnAngle)
            return false;

        if (horizontalInput < 0 && currentYawAngle <= -_jetSkiStats.MaxTurnAngle)
            return false;

        return true;
    }

    void ApplyTurnLean()
    {
        float horizontalInput = _controls.GetHorizontalInput();
        float smoothedInput = Mathf.SmoothStep(0f, 1f, Mathf.Abs(horizontalInput)) * Mathf.Sign(horizontalInput);
        float targetRoll = smoothedInput * _jetSkiStats.MaxLeanAngle;
        // TODO smooth 
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _jetSkiStats.LeanSpeed*0.5f);
        _jetMesh.localRotation = Quaternion.Euler(0f, 0f, -_currentRoll);
    }

    void CorrectPitch()
    {
        if (!_buoyancy.IsUnderWater)
            return;

        float pitch = transform.localEulerAngles.x.NormalizeAngle();
        float pitchError = pitch;

        float correctionStrength = Mathf.InverseLerp(0f, 30f, Mathf.Abs(pitchError)); // tweak 30f as needed
        correctionStrength = Mathf.SmoothStep(0f, 1f, correctionStrength); // makes it soft near zero

        Vector3 flattenTorque = new Vector3(-Mathf.Sign(pitchError), 0f, 0f) * correctionStrength * 30;
        _rigidbody.AddRelativeTorque(flattenTorque, ForceMode.Acceleration);
    }

    void OnDrawGizmosSelected()
    {
        if (_buoyancy.IsUnderWater)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;   

        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2, 0.5f);
    }
}
