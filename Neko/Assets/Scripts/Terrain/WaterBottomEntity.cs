﻿using UnityEngine;

public class WaterBottomEntity : MonoBehaviour
{
    public float WaterBottomLevel;

    private TerrainManager _terrainManager;

    private void Start()
    {
        _terrainManager = TerrainManager.Instance;

        var terrainWidth = (float)_terrainManager.ChunksCount.x * _terrainManager.ChunkSize;
        var terrainHeight = (float)_terrainManager.ChunksCount.y * _terrainManager.ChunkSize;

        transform.position = new Vector3(terrainWidth / 2, WaterBottomLevel, terrainHeight / 2);
        transform.localScale = new Vector3(terrainWidth / 5, 1, terrainHeight / 5);
    }

    private void Update()
    {

    }
}