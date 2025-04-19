using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] float _playerVisibilityDistance = 200f;
    [SerializeField] float _despawnSafeZone = 20f;
    [SerializeField] float _mapWidth = 10;

    [SerializeField, Space] WaterBlock _waterBlock;
    [SerializeField] GameObject[] _startWalls;

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
    List<GameObject[]> _walls = new();
    float _lastTrampolineZ = 0;
    float _lastWaterBlockEndZ = 0;
    float _lastWallZ = 0;
    float _wallLength = 0;

    void Awake()
    {
        _waterBlocks.Add(_waterBlock);
        _lastWaterBlockEndZ = _waterBlock.EndZ;
        _wallLength = _startWalls[0].transform.localScale.z;
        _lastWallZ = _startWalls[0].transform.position.z;
        _walls.Add(new GameObject[2] { _startWalls[0], _startWalls[1] });

        UpdateMap();
    }

    public void UpdateMap()
    {
        UpdateTrampolines();
        UpdateWater();
        UpdateWalls();
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

    public void UpdateWalls()
    {
        while (_lastWallZ < LastVisiblePoint)
        {
            float wallZ = _lastWallZ + _wallLength;
            Vector3 wallPos = new(_mapWidth, _startWalls[0].transform.position.y, wallZ);
            GameObject[] walls = GetFreeWall();
            GameObject leftWall = walls[0];
            GameObject rightWall = walls[1];
            leftWall.name = "LeftWall";
            rightWall.name = "RightWall";
            leftWall.transform.position = wallPos;
            rightWall.transform.position = wallPos + new Vector3(-_mapWidth * 2, 0, 0);
            _lastWallZ = wallZ;
        }
    }

    GameObject[] GetFreeWall()
    {
        foreach (var wallPair in _walls)
        {
            Debug.Log(wallPair[0].transform.position.z + _wallLength / 2.0f);
            if (wallPair[0].transform.position.z + _wallLength / 2.0f < _player.transform.position.z - _despawnSafeZone)
                return wallPair;
        }
        GameObject[] newWalls = new GameObject[2];
        newWalls[0] = Instantiate(_startWalls[0], Vector3.zero, Quaternion.identity, _startWalls[0].transform.parent);
        newWalls[1] = Instantiate(_startWalls[0], Vector3.zero, Quaternion.identity, _startWalls[0].transform.parent);
        _walls.Add(newWalls);
        return newWalls;
    }
}
