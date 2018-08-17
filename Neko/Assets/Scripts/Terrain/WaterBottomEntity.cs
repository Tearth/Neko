using UnityEngine;

public class WaterBottomEntity : MonoBehaviour
{
    public float WaterBottomLevel;

    private void Start()
    {
        var terrainManager = TerrainManager.Instance;

        var terrainWidth = (float)terrainManager.ChunksCount.x * terrainManager.ChunkSize;
        var terrainHeight = (float)terrainManager.ChunksCount.y * terrainManager.ChunkSize;

        transform.position = new Vector3(terrainWidth / 2, WaterBottomLevel, terrainHeight / 2);
        transform.localScale = new Vector3(terrainWidth / 5, 1, terrainHeight / 5);
    }
}