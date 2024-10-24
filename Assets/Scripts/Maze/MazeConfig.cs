using UnityEngine;

[System.Serializable]
public class MazeConfig
{
    [Header("Dimensions")]
    [Tooltip("Denotes the width and height of a single block prefab.")]
    public float blockSize = 1f;

    // These values have to be an odd number
    public int rowCount = 101;
    public int colCount = 31;
    public int emptyZoneRadius = 3;
    
    [Header("Block Distribution")]
    // Governs how many soft blocks are spawned on each end. Linear.
    public float startSoftBlockSpawnRate = 1f;
    public float endSoftBlockSpawnRate = 0f;
    [Space]
    public int numPowerUpBlocks = 3;

    [Header("Toxic Gas Settings")]
    public int[] ventCols;
    public float toxicGasReleaseTime = 3f;
    public float toxicGasSpreadInterval = 1f;

    [Header("Prefabs")]
    public GameObject hardBlockPrefab;
    public GameObject softBlockPrefab;
    public GameObject powerUpBlockPrefab;
    public GameObject exitPrefab;
    public GameObject ventPrefab;
    public ToxicGas toxicGasPrefab;
    public GameObject floorPrefab;
}