using UnityEngine;
using System.Collections.Generic;

public static class MazeGenerator
{
    private enum BlockType
    {
        Hard, Soft, PowerUp, None
    }

    private static Transform _parent;
    private static MazeConfig _config;
    private static BlockType[,] _blocks;

    private static Vector2 BlockPosition(int row, int col)
    {
        // -config.colCount / 2f is added to x to center the maze
        // -row because the lowest row should be at the 
        return new Vector2(-_config.colCount / 2f + col, -row) * _config.blockSize;
    }

    private static GameObject SpawnAt(GameObject prefab, int row, int col)
    {
        GameObject obj = Object.Instantiate(prefab, _parent);
        obj.transform.position = BlockPosition(row, col);
        return obj;
    }

    private static GameObject SpawnBreakableBlock(int row, int col, bool isPowerUpBlock = false)
    {
        GameObject obj = SpawnAt(isPowerUpBlock ? _config.powerUpBlockPrefab : _config.softBlockPrefab, row, col);
        obj.GetComponent<BreakableBlockBase>().Init(row, col);
        return obj;
    }

    public static GameObject[,] Generate(MazeConfig config)
    {
        _config = config;
        
        if (_parent)
        {
            Object.Destroy(_parent.gameObject);
        }

        _parent = new GameObject("Maze").transform;

        _blocks = GenerateMap();

        // make empty zone
        for (int row = 1; row <= 2 * config.emptyZoneRadius - 1; row++)
        {
            for (int col = config.colCount / 2 - config.emptyZoneRadius + 1; col < config.colCount / 2 + config.emptyZoneRadius; col++)
            {
                _blocks[row, col] = BlockType.None;
            }
        }

        for (int row = 1; row < config.rowCount - 1; row++)
        {
            // Decides whether a soft block should be placed,
            // pseudo-randomly chosen according to MazeConfig's soft block distribution rules
            float softBlockChance = SoftBlockChance(row);

            for (int col = 1; col < config.colCount - 1; col++)
            {
                if (_blocks[row, col] == BlockType.None)
                    continue;

                _blocks[row, col] = RandomUtils.Test(softBlockChance) ? BlockType.Soft : BlockType.Hard;
            }
        }

        List<Coords> itemBlockCoords = DecideItemBlockCoords();

        foreach (Coords coords in itemBlockCoords)
        {
            _blocks[coords.Row, coords.Col] = BlockType.PowerUp;
        }

        GameObject[,] maze = new GameObject[config.rowCount, config.colCount];

        for (int row = 0; row < config.rowCount; row++)
        {
            for (int col = 0; col < config.colCount; col++)
            {
                if (_blocks[row, col] == BlockType.None)
                    continue;

                if (_blocks[row, col] == BlockType.Hard)
                    maze[row, col] = SpawnAt(config.hardBlockPrefab, row, col);
                else
                    maze[row, col] = SpawnBreakableBlock(row, col, _blocks[row, col] == BlockType.PowerUp);
            }
        }

        Coords exitCoords = DecideExitCoords();
        SpawnAt(config.exitPrefab, exitCoords.Row, exitCoords.Col);

        foreach (int ventCol in config.ventCols)
        {
            SpawnAt(config.ventPrefab, 1, ventCol);
        }

        GameObject floor = Object.Instantiate(config.floorPrefab, _parent);
        floor.transform.position = new Vector3(-.5f, -config.rowCount * config.blockSize / 2 +.5f, 1f);
        floor.transform.localScale = new Vector2(config.colCount, config.rowCount) * config.blockSize;

        return maze;
    }

    private static BlockType[,] GenerateMap()
    {
        BlockType[,] map = new BlockType[_config.rowCount, _config.colCount];

        void GenerateRecursive(int row, int col)
        {
            map[row, col] = BlockType.None;

            int[] dirs = { 0, 1, 2, 3 };
            RandomUtils.Shuffle(ref dirs);

            foreach (int dir in dirs)
            {
                int nRow = row + Direction.directions[dir].rowDelta * 2;
                int nCol = col + Direction.directions[dir].colDelta * 2;

                // within bounds, and not visited already
                if (nRow >= 0 && nRow < _config.rowCount && nCol >= 0 && nCol < _config.colCount 
                    && map[nRow, nCol] != BlockType.None)
                {
                    map[row + Direction.directions[dir].rowDelta, col + Direction.directions[dir].colDelta] = BlockType.None; // make path
                    GenerateRecursive(nRow, nCol);
                }
            }
        }

        GenerateRecursive(1, 1);

        return map;
    }

    private static float SoftBlockChance(int row)
    {
        return Mathf.Lerp(_config.startSoftBlockSpawnRate, _config.endSoftBlockSpawnRate, (float)row / _config.rowCount);
    }

    private static List<Coords> DecideItemBlockCoords()
    {
        List<Coords> itemBlockCoords = new List<Coords>();

        int row = _config.rowCount / (_config.numPowerUpBlocks + 1);

        for (int i = 0; i < _config.numPowerUpBlocks; i++)
        {
            List<int> candidateCols = new List<int>();

            bool nextRowFlag = false;
            if (!GetPowerUpBlockCandidateCols(row, ref candidateCols))
            {
                // retry on next row
                nextRowFlag = true;
                GetPowerUpBlockCandidateCols(row + 1, ref candidateCols);
            }

            int placeRow = nextRowFlag ? row + 1 : row;

            itemBlockCoords.Add(new Coords(placeRow, candidateCols[Random.Range(0, candidateCols.Count)]));

            row += _config.rowCount / (_config.numPowerUpBlocks + 1);
        }

        return itemBlockCoords;
    }

    private static bool GetPowerUpBlockCandidateCols(int row, ref List<int> candidates)
    {
        bool found = false;

        for (int col = 1; col < _config.colCount - 1; col++)
        {
            if (_blocks[row, col] != BlockType.None && IsReachable(row, col))
            {
                candidates.Add(col);
                found = true;
            }
        }

        return found;
    }

    // Whether a coord has at least one empty adjacent coord
    private static bool IsReachable(int row, int col)
    {
        foreach (Direction direction in Direction.directions)
        {
            int nRow = row + direction.rowDelta;
            int nCol = col + direction.colDelta;
            
            if (_blocks[nRow, nCol] == BlockType.None)
                return true;
        }

        return false;
    }

    private static Coords DecideExitCoords()
    {
        List<Coords> candidates = new List<Coords>();

        for (int col = 1; col < _config.colCount - 1; col++)
        {
            if (_blocks[_config.rowCount - 2, col] == BlockType.None)
                candidates.Add(new Coords(_config.rowCount - 2, col));
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}
