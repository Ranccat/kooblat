using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    private MazeConfig _config;
    private GameObject[,] _blocks;

    public int RowCount => _config.rowCount;
    public int ColCount => _config.colCount;

    public Vector2 PlayerStartPos => GetPosition(_config.emptyZoneRadius, _config.colCount / 2);

    // for camera
    public Vector2 TopLeftBounds => GetPosition(0, 0) + new Vector2(-1, 1) * _config.blockSize / 2;
    public Vector2 BottomRightBounds => GetPosition(_config.rowCount - 1, _config.colCount - 1) + new Vector2(1, -1) * _config.blockSize / 2;

    private Transform _toxicGasParent;
    private ToxicGas[,] _toxicGasMap;
    private Queue<ToxicGas> _toxicGasSpreadQueue;
    private Queue<Coords> _toxicGasToBeSpawned; // Queue of coordinates in which additional gas tiles should be spawned, caused by soft blocks breaking

    #region Public Controlled Methods
    public void Init(MazeConfig config)
    {
        _config = config;

        _blocks = MazeGenerator.Generate(config);

        _toxicGasParent = new GameObject("Toxic Gas").transform;
        _toxicGasSpreadQueue = new Queue<ToxicGas>();
        _toxicGasMap = new ToxicGas[config.rowCount, config.colCount];
        _toxicGasToBeSpawned = new Queue<Coords>();
    }

    public void StartToxicGas()
    {
        StartCoroutine(ToxicGasRoutine());
    }
    #endregion

    #region Public Helper Methods
    public GameObject GetBlockAt(int row, int col)
    {
        if (IsOutOfBounds(row, col))
            return null;

        return _blocks[row, col];
    }

    public GameObject GetBlockAt(Coords coords)
    {
        return GetBlockAt(coords.Row, coords.Col);
    }

    public Vector2 GetPosition(int row, int col)
    {
        return new Vector2(-_config.colCount / 2f + col, -row) * _config.blockSize;
    }

    public Vector2 GetPosition(Coords coords)
    {
        return GetPosition(coords.Row, coords.Col);
    }

    public Coords GetCoords(Vector2 position)
    {
        return new Coords((int)(-position.y / _config.blockSize), (int)(position.x / _config.blockSize + _config.colCount / 2) + 1);
    }

    public bool IsOutOfBounds(int row, int col)
    {
        return row < 0 || row >= RowCount || col < 0 || col >= ColCount;
    }

    public bool IsOutOfBounds(Coords coords)
    {
        return IsOutOfBounds(coords.Row, coords.Col);
    }

    public bool IsOutOfBounds(Vector2 position)
    {
        return IsOutOfBounds(GetCoords(position));
    }
    #endregion

    #region Toxic Gas
    private IEnumerator ToxicGasRoutine()
    {
        yield return new WaitForSeconds(_config.toxicGasReleaseTime);

        ReleaseToxicGas();

        while (true)
        {
            yield return new WaitForSeconds(_config.toxicGasSpreadInterval);

            SpreadToxicGas();
        }
    }

    private ToxicGas SpawnToxicGas(int row, int col)
    {
        ToxicGas toxicGas = _toxicGasMap[row, col] = Instantiate(_config.toxicGasPrefab, _toxicGasParent);
        toxicGas.transform.position = GetPosition(row, col);
        toxicGas.Init(row, col);

        _toxicGasSpreadQueue.Enqueue(toxicGas);

        return toxicGas; // half state
    }

    private void ReleaseToxicGas()
    {
        foreach (int ventCol in _config.ventCols)
        {
            SpawnToxicGas(1, ventCol);
        }
    }

    private void SpreadToxicGas()
    {
        int qSize = _toxicGasSpreadQueue.Count;
        while (qSize-- > 0)
        {
            ToxicGas toxicGas = _toxicGasSpreadQueue.Dequeue();

            foreach (Direction direction in Direction.directions)
            {
                int nRow = toxicGas.row + direction.rowDelta;
                int nCol = toxicGas.col + direction.colDelta;

                if (_blocks[nRow, nCol] == null && !_toxicGasMap[nRow, nCol])
                    SpawnToxicGas(nRow, nCol);
            }

            toxicGas.SetFullIntensity();
        }

        while (_toxicGasToBeSpawned.Count > 0)
        {
            int row = _toxicGasToBeSpawned.Peek().Row;
            int col = _toxicGasToBeSpawned.Peek().Col;

            _toxicGasToBeSpawned.Dequeue();

            if (!_toxicGasMap[row, col])
                SpawnToxicGas(row, col);
        }
    }

    // to update Toxic Gas BFS
    public void NotifyBlockRemoved(int row, int col)
    {
        foreach (Direction direction in Direction.directions)
        {
            int nRow = row + direction.rowDelta;
            int nCol = col + direction.colDelta;

            if (_toxicGasMap[nRow, nCol] && _toxicGasMap[nRow, nCol].isFullIntensity)
            {
                _toxicGasToBeSpawned.Enqueue(new Coords(row, col));
                return;
            }
        }
    }
    #endregion
}
