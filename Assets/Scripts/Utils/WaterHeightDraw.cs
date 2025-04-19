using Bitgem.VFX.StylisedWater;
using UnityEngine;

public class WaterHeightDraw : MonoBehaviour
{
    [SerializeField] WaterVolumeHelper _waterVolumeHelper;

    [SerializeField] float _xOffset, _zOffset;
    [SerializeField] int _xSize, _zSize;
    [SerializeField] float _heightOffset;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int x = 0; x < _xSize; x++)
        {
            for (int z = 0; z < _zSize; z++)
            {
                Vector3 position = transform.position + new Vector3(x * _xOffset, 0, z * _zOffset);
                float waterHeight = _waterVolumeHelper.GetHeight(position) ?? position.y;
                Gizmos.DrawSphere(new Vector3(position.x, waterHeight + _heightOffset, position.z), 0.1f);
            }
        }
    }
}
