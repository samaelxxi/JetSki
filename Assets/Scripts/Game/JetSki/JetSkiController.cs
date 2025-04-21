using UnityEngine;
using UnityEngine.Animations;

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

        CorrectPitchAndRoll();
        DumpAngularSpeed();
    }

    void DumpAngularSpeed()
    {
        // jet goes crazy sometimes, so we need to limit the angular velocity
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
        else // slow down when going backwards
            _currentSpeed *= Mathf.MoveTowards(_currentSpeed, _jetSkiStats.MinSpeed, _jetSkiStats.Deceleration * Time.fixedDeltaTime);

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 acceleration = flatForward * _currentSpeed;
        _rigidbody.AddForce(acceleration, ForceMode.Acceleration);
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
    {  // show roll only as visual feedback, not as a real physics effect
        float horizontalInput = _controls.GetHorizontalInput();
        float smoothedInput = Mathf.SmoothStep(0f, 1f, Mathf.Abs(horizontalInput)) * Mathf.Sign(horizontalInput);
        float targetRoll = smoothedInput * _jetSkiStats.MaxLeanAngle;
        _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * _jetSkiStats.LeanSpeed*0.5f);
        _jetMesh.localRotation = Quaternion.Euler(0f, 0f, -_currentRoll);
    }

    void CorrectPitchAndRoll()
    {
        if (!_buoyancy.IsUnderWater)
            return;

        ApplyFlatteningTorque(Axis.X, 30f, 30f); // Pitch correction
        ApplyFlatteningTorque(Axis.Z, 30f, 90f); // Roll correction
    }

    void ApplyFlatteningTorque(Axis axis, float maxErrorAngle, float maxTorque)
    {
        float angle = axis == Axis.X ? transform.localEulerAngles.x : transform.localEulerAngles.z;
        float error = angle.NormalizeAngle();

        float correctionStrength = Mathf.InverseLerp(0f, maxErrorAngle, Mathf.Abs(error));
        correctionStrength = Mathf.SmoothStep(0f, 1f, correctionStrength);

        Vector3 torque = axis switch
        {
            Axis.X => new Vector3(-Mathf.Sign(error), 0f, 0f),
            Axis.Z => new Vector3(0f, 0f, -Mathf.Sign(error)),
            _ => Vector3.zero
        };

        _rigidbody.AddRelativeTorque(correctionStrength * maxTorque * torque, ForceMode.Acceleration);
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
