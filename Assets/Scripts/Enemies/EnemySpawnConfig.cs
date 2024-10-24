using UnityEngine;

[System.Serializable]
public struct SpawnTypeAndCount
{
    public Enemy enemy;
    public int count;
}

[System.Serializable]
public class EnemySpawnConfig
{
    public float initialDelay = 10f;
    public float spawnInterval = 5f;
    public int spawnCount = 3;
    [Space()]
    public SpawnTypeAndCount[] spawnTypes;
}
