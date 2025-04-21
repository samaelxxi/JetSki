using UnityEngine;

public class PilotLeanRig : MonoBehaviour
{
    [SerializeField] Transform _leanTarget;
    [SerializeField] Transform _restTarget;
    [SerializeField] RangeValue _xRange = new RangeValue(-0.5f, 0.5f);
    [SerializeField] RangeValue _zRange = new RangeValue(-0.5f, 0.5f);
    [SerializeField] float _xLeanSmoothTime = 20f;
    [SerializeField] float _zLeanSmoothTime = 20f;

    float _currentX, _currentZ;
    float smoothX, smoothZ;
    IJetSkiControls _controls;


    public void SetControls(IJetSkiControls controls)
    {
        _controls = controls;
    }

    void Update()
    {
        float xInput = _controls.GetHorizontalInput();
        float zInput = _controls.GetVerticalInput();
        float middleX = _restTarget.localPosition.x;
        float middleZ = _restTarget.localPosition.z;

        float targetX, targetZ;
        if (Mathf.Abs(xInput) < 0.001f)
            targetX = _restTarget.localPosition.x;
        else
            targetX = xInput > 0 ? middleX + (_xRange.Max - middleX) * xInput 
                                 : middleX - (_xRange.Min - middleX) * xInput;

        if (Mathf.Abs(zInput) < 0.001f)
            targetZ = middleZ;
        else
            targetZ = zInput > 0 ? middleZ + (_zRange.Max - middleZ) * zInput 
                                 : middleZ - (_zRange.Min - middleZ) * zInput;

        _currentX = Mathf.SmoothDamp(_currentX, targetX, ref smoothX, _xLeanSmoothTime * Time.deltaTime);
        _currentZ = Mathf.SmoothDamp(_currentZ, targetZ, ref smoothZ, _zLeanSmoothTime * Time.deltaTime);

        Vector3 targetPosition = new(_currentX, _restTarget.localPosition.y, _currentZ);
        _leanTarget.localPosition = targetPosition;
    }
}
