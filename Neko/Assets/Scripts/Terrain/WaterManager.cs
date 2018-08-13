using UnityEngine;

public class WaterManager : MonoBehaviourSingleton<WaterManager>
{
    public float WaterLevel;

    private TerrainManager _terrainManager;

    private void Start()
    {
        _terrainManager = TerrainManager.Instance;

        var terrainWidth = (float)_terrainManager.ChunksCount.x * _terrainManager.ChunkSize;
        var terrainHeight = (float)_terrainManager.ChunksCount.y * _terrainManager.ChunkSize;

        transform.position = new Vector3(terrainWidth / 2, WaterLevel, terrainHeight / 2);
        transform.localScale = new Vector3(terrainWidth, 1, terrainHeight);
    }

    private void Update()
    {

    }
}