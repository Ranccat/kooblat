using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private StageConfig _stageConfig;

    private static Game _instance;
    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Game>();

                if (_instance == null)
                    Debug.LogError("Game Instance Missing!");
            }

            return _instance;
        }
    }

    private Player _player;
    public static Player Player { get { return Instance._player; } }

    private Maze _maze;
    public static Maze Maze { get { return Instance._maze; } }

    private EnemySpawner _enemySpawner;

    private UIManagerEx _ui;
    public static UIManagerEx UI { get { return Instance._ui; } }

    private float _elapsedTime;
    private float _timerUpdateInterval = 1f;
    private float _nextTimerUpdateTime = 0f;
    private static int _score = 0;

    private void Awake()
    {
        _maze = FindObjectOfType<Maze>();
        _player = FindObjectOfType<Player>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _ui = FindObjectOfType<UIManagerEx>();
    }

    private void Start()
    {
        _maze.Init(_stageConfig.mazeConfig);
        AStar.Configure(_maze);

        _enemySpawner.Init(_stageConfig.enemySpawnConfig);
        _ui.InitPlayerUI();

        _player.transform.position = Maze.PlayerStartPos;

        _maze.StartToxicGas();
        _enemySpawner.StartSpawning();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (Time.time >= _nextTimerUpdateTime)
        {
            _ui.UpdateTimer(_elapsedTime);
            _nextTimerUpdateTime = Time.time + _timerUpdateInterval;
        }
    }

    public void OnMazeExitReached()
    {
        print("Lol congrats you beat the level, bitch");
    }

    public static void AddScore(int amount)
    {
        _score += amount;
        UI.UpdateScore(_score);
    }
}
