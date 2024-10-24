using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnMarginDistance = 1f;

    private EnemySpawnConfig _config;
    private Queue<Enemy> _enemyPool;

    private Maze _maze;
    private PlayerCamera _playerCam;

    public void Init(EnemySpawnConfig config)
    {
        _config = config;

        InitPool();

        _maze = Game.Maze;
        _playerCam = Camera.main.GetComponent<PlayerCamera>();
    }

    private void InitPool()
    {
        List<Enemy> enemyList = new List<Enemy>();

        foreach (SpawnTypeAndCount spawnType in _config.spawnTypes)
        {
            for (int i = 0; i < spawnType.count; i++)
            {
                Enemy enemy = Instantiate(spawnType.enemy);
                enemy.gameObject.SetActive(false);
                enemyList.Add(enemy);
            }
        }

        RandomUtils.Shuffle(ref enemyList);

        _enemyPool = new Queue<Enemy>(enemyList);
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        print("EnemySpawnRoutine Begin");

        yield return new WaitForSeconds(_config.initialDelay);

        while (_enemyPool.Count > 0)
        {
            int spawnCount = Mathf.Min(_config.spawnCount, _enemyPool.Count);
            List<Vector2> spawnPositions = DecideEnemySpawnPositions(spawnCount);

            for (int i = 0; i < spawnCount; i++)
            {
                Enemy enemy = _enemyPool.Dequeue();
                enemy.transform.position = spawnPositions[i];
                enemy.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(_config.spawnInterval);
        }

        print("EnemySpawnRoutine Terminate");
    }

    private List<Vector2> DecideEnemySpawnPositions(int count)
    {
        List<Coords> cameraBoundaryCoords = GetCameraBoundaryCoords();

        if (cameraBoundaryCoords.Count < count)
            Debug.LogError($"Failed to find {count} positions to spawn enemies on!");

        RandomUtils.Shuffle(ref cameraBoundaryCoords);

        List<Vector2> spawnPositions = new List<Vector2>(count);
        for (int i = 0; i < count; i++)
            spawnPositions.Add(_maze.GetPosition(cameraBoundaryCoords[i]));

        return spawnPositions;
    }

    private List<Coords> GetCameraBoundaryCoords()
    {
        Coords camTopLeftCoords = _maze.GetCoords(_playerCam.TopLeftBounds + new Vector2(-1, 1) * _spawnMarginDistance);
        Coords camBottomRightCoords = _maze.GetCoords(_playerCam.BottomRightBounds + new Vector2(1, -1) * _spawnMarginDistance);

        int topRow = camTopLeftCoords.Row;
        int leftCol = camTopLeftCoords.Col;
        int bottomRow = camBottomRightCoords.Row;
        int rightCol = camBottomRightCoords.Col;
        int height = bottomRow - topRow + 1;
        int width = rightCol - leftCol + 1;

        List<Coords> boundaryCoords = new List<Coords>();
        boundaryCoords.AddRange(GetEmptyCoordsInRange(topRow, leftCol, 0, 1, width - 1));
        boundaryCoords.AddRange(GetEmptyCoordsInRange(topRow, rightCol, 1, 0, height - 1));
        boundaryCoords.AddRange(GetEmptyCoordsInRange(bottomRow, rightCol, 0, -1, width - 1));
        boundaryCoords.AddRange(GetEmptyCoordsInRange(bottomRow, leftCol, -1, 0, height - 1));

        return boundaryCoords;
    }

    private List<Coords> GetEmptyCoordsInRange(int startRow, int startCol, int dRow, int dCol, int length)
    {
        List<Coords> coords = new List<Coords>();

        int row = startRow;
        int col = startCol;
        for (int i = 0; i < length; i++)
        {
            if (!_maze.IsOutOfBounds(row, col) && _maze.GetBlockAt(row, col) == null)
                coords.Add(new Coords(row, col));
            row += dRow;
            col += dCol;
        }

        return coords;
    }
}
