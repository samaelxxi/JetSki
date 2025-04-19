using UnityEngine;
using Bitgem.VFX.StylisedWater;

public class WaterBlock : MonoBehaviour
{
    [SerializeField] Transform _waterBlockMarker;
    [SerializeField] WaterVolumeHelper _waterVolumeHelper;

    public float StartZ => transform.position.z;
    public float EndZ => transform.position.z + _blockSizeZ;

    float _blockSizeZ;

    void Awake()
    {
        _blockSizeZ = _waterBlockMarker.localScale.z;
    }

    public bool IsPlayerOnBlock(float playerZ)
    {
        return playerZ > StartZ && playerZ < EndZ;
    }

    public float GetWaterHeight(Vector3 position)
    {
        return _waterVolumeHelper.GetHeight(position) ?? position.y;
    }
}
