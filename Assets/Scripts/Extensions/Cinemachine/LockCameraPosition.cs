using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Constrains the camera's position within specified bounds on selected axes.
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")]
public class LockCameraPosition : CinemachineExtension
{
    [Header("Constraints")]
    [SerializeField] private bool constraintX = true;
    [SerializeField] private bool constraintY = true;
    [SerializeField] private bool constraintZ = true;

    [Header("Bounding Box Corners")]
    [SerializeField] private Vector3 positionA;
    [SerializeField] private Vector3 positionB;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, 
        ref CameraState state, 
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body)
            return;

        state.RawPosition = ClampPosition(state.RawPosition);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        Vector3 min = Vector3.Min(positionA, positionB);
        Vector3 max = Vector3.Max(positionA, positionB);

        if (constraintX) position.x = Mathf.Clamp(position.x, min.x, max.x);
        if (constraintY) position.y = Mathf.Clamp(position.y, min.y, max.y);
        if (constraintZ) position.z = Mathf.Clamp(position.z, min.z, max.z);

        return position;
    }
}