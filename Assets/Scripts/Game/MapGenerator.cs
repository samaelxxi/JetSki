using System.Collections;
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
    [SerializeField] Transform _floor;

    [SerializeField, Header("CCCCoins")] PigCoin _coinPrefab;
    [SerializeField] float _coinRotationSpeed = 60f;
    [SerializeField] float _waterCoinY = 2.5f;
    [SerializeField] float _coinXRange = 4f;

    [SerializeField, Range(0, 1)] float _trampolineCoinChance = 0.5f;
    [SerializeField, Range(0, 1)] float _additionalCoinChance = 0.3f;
    [SerializeField, Range(0, 1)] float _trampolineadditionalCoinChance = 0.6f;
    [SerializeField, Range(0, 1)] float _trampolineCoinsDist = 3f;

    [SerializeField] int _maxCoinsInARow = 4;
    [SerializeField, Min(0.01f)] float _meanCoinPerCell = 0.1f;
    [SerializeField] int _coinCellSize = 10;


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
    List<PigCoin> _coins = new();
    Transform _coinsParent;
    float _lastTrampolineZ = 0;
    float _lastWaterBlockEndZ = 0;
    float _lastWallZ = 0;
    float _wallLength = 0;
    float _lastCoinZ = 0;
    int _noCoinsCells = 0;


    void Awake()
    {
        _waterBlocks.Add(_waterBlock);
        _lastWaterBlockEndZ = _waterBlock.EndZ;
        _wallLength = _startWalls[0].transform.localScale.z;
        _lastWallZ = _startWalls[0].transform.position.z;
        _walls.Add(new GameObject[2] { _startWalls[0], _startWalls[1] });
        _coinsParent = new GameObject("Coins").transform;
        _coinsParent.SetParent(transform);

        UpdateMap();
        StartCoroutine(CoinsRotation());
    }

    public void UpdateMap()
    {
        UpdateTrampolines();
        UpdateWater();
        UpdateWalls();
        UpdateCoins();

        // whatever, just move it along the player, needed only for water depth
        _floor.position = new Vector3(_floor.position.x, _floor.position.y, _player.transform.position.z);
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

            if (Random.value < _trampolineCoinChance)
                AddTrampolineCoins(trampolinePos);
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

    void AddTrampolineCoins(Vector3 trampolinePos)
    {
        int coins = 1;
        while (coins < _maxCoinsInARow && Random.value < _trampolineadditionalCoinChance)
            coins++;

        Vector3 startPos = trampolinePos + new Vector3(0, 3, 5);
        for (int i = 0; i < coins; i++)
        {
            PigCoin coin = GetFreeCoin();
            coin.Spawn();
            coin.transform.position = startPos + new Vector3(0, 0, i * _trampolineCoinsDist);
        }
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

    void UpdateWalls()
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
            if (wallPair[0].transform.position.z + _wallLength / 2.0f < _player.transform.position.z - _despawnSafeZone)
                return wallPair;
        }
        GameObject[] newWalls = new GameObject[2];
        newWalls[0] = Instantiate(_startWalls[0], Vector3.zero, Quaternion.identity, _startWalls[0].transform.parent);
        newWalls[1] = Instantiate(_startWalls[0], Vector3.zero, Quaternion.identity, _startWalls[0].transform.parent);
        _walls.Add(newWalls);
        return newWalls;
    }

    void UpdateCoins()
    {
        while (_lastCoinZ < LastVisiblePoint)
        {
            _lastCoinZ += _coinCellSize;
            int coinsToSpawn = HowManyCoinsShouldAddAtCurrentZ();
            for (int i = 0; i < coinsToSpawn; i++)
            {
                Vector3 coinPos = new(Random.Range(-_trampolineXRange, _trampolineXRange), _waterCoinY, _lastCoinZ);
                PigCoin coin = GetFreeCoin();
                coin.Spawn();
                coin.transform.position = coinPos;
            }
        }
    }

    int HowManyCoinsShouldAddAtCurrentZ()
    {
        float chance = Random.value;
        float lambda = 1.0f / _meanCoinPerCell;
        float currentChance = 1f - Mathf.Exp(-_noCoinsCells / lambda);
        if (chance < currentChance)
        {
            _noCoinsCells = 0;
            int coins = 1;
            while (coins < _maxCoinsInARow && Random.value < _additionalCoinChance)
                coins++;

            return coins;
        }
        else
        {
            _noCoinsCells++;
            return 0;
        }
    }

    PigCoin GetFreeCoin()
    {
        foreach (var coin in _coins)
            if (!coin.IsSpawned || coin.transform.position.z < _player.transform.position.z - _despawnSafeZone)
                return coin;

        PigCoin newCoin = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity, _coinsParent.transform);
        _coins.Add(newCoin);
        return newCoin;
    }

    IEnumerator CoinsRotation()
    {
        while (true)
        {
            foreach (var coin in _coins)
                coin.transform.rotation = Quaternion.Euler(0, Time.time * _coinRotationSpeed, 0);

            yield return null;
        }
    }
}
