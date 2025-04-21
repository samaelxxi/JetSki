using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    [SerializeField] Transform _buoyancyPoint;

    [Header("Buoyancy Settings")]
    [SerializeField] float _underWaterDrag = 3f;
    [SerializeField] float _underWaterAngularDrag = 1f;
    [SerializeField] float _airDrag = 0.1f;
    [SerializeField] float _airAngularDrag = 0.05f;
    [SerializeField] float _buoyancyStrength = 20f;
    [SerializeField] float _buoyancyDamping = 0.1f;
    [SerializeField] float _waterLevelOffset = 0f;
    [SerializeField] float _maxDepth = 2f;

    public bool IsUnderWater => _isUnderWater;

    Rigidbody _rigidbody;
    bool _isUnderWater;
    WaterBlock _currentWaterBlock;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyBuoyancy();
    }

    void ApplyBuoyancy()
    {
        float waterHeight = GetWaterHeight();
        float depth = waterHeight - _buoyancyPoint.position.y;

        if (depth > 0f)
        {
            float springForce = Mathf.Clamp(depth, 0, _maxDepth) * _buoyancyStrength;
            Vector3 pointVelocity = _rigidbody.GetPointVelocity(_buoyancyPoint.position);

            float dampingForce = -pointVelocity.y * _buoyancyDamping;
            float totalForce = springForce + dampingForce;
            totalForce *= _rigidbody.mass;

            _rigidbody.AddForce(Vector3.up * totalForce, ForceMode.Force);
        }

        bool newUnderWaterState = depth > 0f;
        if (newUnderWaterState != _isUnderWater)
            UpdateDragState(newUnderWaterState);
    }

    float GetWaterHeight()
    {
        Vector3 pos = _buoyancyPoint.position;
        if (_currentWaterBlock == null || !_currentWaterBlock.IsPlayerOnBlock(pos.z))
            _currentWaterBlock = ServiceLocator.Get<GameLevel>().GetWaterBlockAtZ(pos.z);

        if (_currentWaterBlock != null)
        {
            // it calculates height somewhat strange on plugin side, so we need to add offset to it
            return _currentWaterBlock.GetWaterHeight(pos) + _waterLevelOffset;
        }

        return _waterLevelOffset; // Default water level if no block is found
    }

    void UpdateDragState(bool enteringWater)
    {
        if (enteringWater)
        {
            _rigidbody.linearDamping = _underWaterDrag;
            _rigidbody.angularDamping = _underWaterAngularDrag;
        }
        else
        {
            _rigidbody.linearDamping = _airDrag;
            _rigidbody.angularDamping = _airAngularDrag;
        }
        _isUnderWater = enteringWater;
    }


    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_buoyancyPoint.position, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_buoyancyPoint.position.SetY(GetWaterHeight()), 0.5f);
    }
}
