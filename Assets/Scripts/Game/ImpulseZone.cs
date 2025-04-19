using UnityEngine;

public class ImpulseZone : MonoBehaviour
{
    [SerializeField] LayerMask _interactionLayerMask;
    [SerializeField] float _forceMultiplier = 10f;
    [SerializeField] Vector3 _forceDirection = Vector3.forward;
    [SerializeField] bool _adjustByMass = true;

    void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & _interactionLayerMask) == 0)
            return;

        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 force = _forceDirection.normalized * _forceMultiplier;
            if (_adjustByMass)
                force *= rb.mass;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
