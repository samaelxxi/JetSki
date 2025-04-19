using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] float _playerVisibilityDistance = 200f;
    [SerializeField] float _despawnSafeZone = 20f;

    [SerializeField, Space()] WaterBlock _waterBlock;

    [SerializeField, Header("Trampolines")] GameObject _trampolinePrefab;
    [SerializeField] Transform _trampolineParent;
    [SerializeField] float _trampolineY = 0.5f;
    [SerializeField] UniformValue _trampolineAngle = new(-20, 10);
    [SerializeField] UniformValue _trampolineDistance = new(50, 10);
    [SerializeField] float _trampolineXRange = 4f;


    public IEnumerable<WaterBlock> WaterBlocks => _waterBlocks;

    float LastVisiblePoint => _player.transform.position.z + _playerVisibilityDistance;

    List<GameObject> _trampolines = new();
    List<WaterBlock> _waterBlocks = new();
    float _lastTrampolineZ = 0;
    float _lastWaterBlockEndZ = 0;

    void Awake()
    {
        _waterBlocks.Add(_waterBlock);
        _lastWaterBlockEndZ = _waterBlock.EndZ;

        UpdateMap();
    }

    public void UpdateMap()
    {
        UpdateTrampolines();
        UpdateWater();
    }

    void UpdateTrampolines()
    {
        while (_lastTrampolineZ < LastVisiblePoint)
        {
            float trampolineZ = _lastTrampolineZ + _trampolineDistance.GetRandomValue();
            Vector3 trampolinePos = new(Random.Range(-_trampolineXRange, _trampolineXRange), _trampolineY, trampolineZ);
            var trampolineRotation = Quaternion.Euler(_trampolineAngle.GetRandomValue(), 0, 0);
            GameObject trampoline = GetFreeTrampoline();
            trampoline.transform.SetPositionAndRotation(trampolinePos, trampolineRotation);
            _lastTrampolineZ = trampolineZ;
        }
    }

    GameObject GetFreeTrampoline()
    {
        foreach (var trampoline in _trampolines)
        {
            if (trampoline.transform.position.z < _player.transform.position.z - _despawnSafeZone)
                return trampoline;
        }
        GameObject newTrampoline = Instantiate(_trampolinePrefab, Vector3.zero, Quaternion.identity, _trampolineParent);
        _trampolines.Add(newTrampoline);
        return newTrampoline;
    }

    void UpdateWater()
    {
        while (_lastWaterBlockEndZ < LastVisiblePoint)
        {
            Vector3 newWaterBlockPos = _waterBlock.transform.position;
            newWaterBlockPos.z = _lastWaterBlockEndZ;
            WaterBlock waterBlock = GetFreeWaterBlock();
            waterBlock.transform.position = newWaterBlockPos;
            _lastWaterBlockEndZ = waterBlock.EndZ;
        }
    }

    WaterBlock GetFreeWaterBlock()
    {
        foreach (var waterBlock in _waterBlocks)
            if (waterBlock.EndZ < _player.transform.position.z - _despawnSafeZone)
                return waterBlock;

        WaterBlock newWaterBlock = Instantiate(_waterBlock, Vector3.zero, Quaternion.identity, _waterBlock.transform.parent);
        _waterBlocks.Add(newWaterBlock);
        return newWaterBlock;
    }
}
