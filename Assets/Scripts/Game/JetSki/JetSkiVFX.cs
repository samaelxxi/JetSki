using UnityEngine;

public class JetSkiVFX : MonoBehaviour
{
    [SerializeField] JetSkiController _jetSkiController;
    [SerializeField] ParticleSystem[] _trails;

    [SerializeField, Header("Side splash")] ParticleSystem[] _sideSplashes;
    [SerializeField] float _minYVelocityForSplash = 5;
    [SerializeField] RangeValue _splashCountRange = new RangeValue(10, 100);
    [SerializeField] RangeValue _splashMagnitudeRange = new RangeValue(10, 35);


    bool _prevUnderWater = false;

    void Update()
    {
        bool _isUnderWater = _jetSkiController.IsUnderWater;
    
        foreach (var trail in _trails)
        {
            if (_isUnderWater)
                trail.Play();
            else
                trail.Stop();
        }

        if (_isUnderWater && !_prevUnderWater)
        {
            float fallDir = Mathf.Sign(_jetSkiController.Rigidbody.linearVelocity.y);
            float fallSpeed = Mathf.Abs(_jetSkiController.Rigidbody.linearVelocity.y);

            if (fallDir < 0 && fallSpeed > _minYVelocityForSplash)
            {
                float fallMagn = _jetSkiController.Rigidbody.linearVelocity.magnitude;

                int splashCount = (int)MathExtensions.LinearMap(fallMagn, _splashMagnitudeRange.Min, _splashMagnitudeRange.Max, 
                                                                           _splashCountRange.Min, _splashCountRange.Max);
                foreach (var splash in _sideSplashes)
                    splash.Emit(splashCount);
            }
        }

        _prevUnderWater = _isUnderWater;
    }

}
