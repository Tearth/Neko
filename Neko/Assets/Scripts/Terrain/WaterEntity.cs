using UnityEngine;

public class WaterEntity : MonoBehaviour
{
    public float WaterLevel;

    private void Start()
    {
        var terrainManager = TerrainManager.Instance;

        var terrainWidth = (float)terrainManager.ChunksCount.x * terrainManager.ChunkSize;
        var terrainHeight = (float)terrainManager.ChunksCount.y * terrainManager.ChunkSize;

        transform.position = new Vector3(terrainWidth / 2, WaterLevel, terrainHeight / 2);
        transform.localScale = new Vector3(terrainWidth, 1, terrainHeight);
    }
}