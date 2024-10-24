using UnityEngine;

public class TestEnemySpawner : MonoBehaviour
{
    [SerializeField] private float _initDelay = 10f;
    [SerializeField] private float _spawnInterval = 10f;
    [SerializeField] private int _maxCount = 20;

    [SerializeField] private GameObject _enemyPrefab;

    private float _spawnTimer;
    private int _spawnedCount;

    private void Start()
    {
        _spawnTimer = _initDelay;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);

            if (++_spawnedCount >= _maxCount)
                Destroy(gameObject);

            _spawnTimer = _spawnInterval;
        }
    }
}
