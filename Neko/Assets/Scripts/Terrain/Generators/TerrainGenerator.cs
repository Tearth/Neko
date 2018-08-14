using System;
using UnityEngine;

public class TerrainGenerator
{
    private TerrainManager _terrainManager;

    public TerrainGenerator()
    {
        _terrainManager = TerrainManager.Instance;
    }

    public VoxelData[,,] Generate(Vector2Int position)
    {
        var heightMap = new VoxelData[_terrainManager.ChunkSize, _terrainManager.ChunkSize, _terrainManager.SpaceHeight];

        var perlinXPos = position.x * _terrainManager.PerlinNoiseScale;
        var perlinYPos = position.y * _terrainManager.PerlinNoiseScale;
        var step = 1f * _terrainManager.PerlinNoiseScale / _terrainManager.ChunkSize;

        for (var x = 0; x < _terrainManager.ChunkSize; x++)
        {
            for (var y = 0; y < _terrainManager.ChunkSize; y++)
            {
                var perlinX = perlinXPos + x * step;
                var perlinY = perlinYPos + y * step;
                var edgeRatio = GetEdgeRatio(position, new Vector2Int(x, y), _terrainManager.ChunksCount, _terrainManager.ChunkSize);

                var topVoxelHeight = (int)((Mathf.Clamp(Mathf.PerlinNoise(perlinX, perlinY), 0, 1) * _terrainManager.MaxTerrainHeight) * edgeRatio) + _terrainManager.BaseTerrainHeight;

                var topVoxelData = new VoxelData();
                topVoxelData.Type = topVoxelHeight > 7 ? VoxelType.Dirt : VoxelType.Sand;

                heightMap[x, y, topVoxelHeight] = topVoxelData;

                for (var z = 0; z < topVoxelHeight; z++)
                {
                    var voxelData = new VoxelData();
                    voxelData.Type = VoxelType.Sand;

                    heightMap[x, y, z] = voxelData;
                }
            }
        }

        return heightMap;
    }

    private float GetEdgeRatio(Vector2Int chunkPosition, Vector2Int voxelPosition, Vector2Int chunksCount, int chunkSize)
    {
        var xRatio = 1f;
        var yRatio = 1f;

        if (chunkPosition.x == 0)
        {
            xRatio = (float)voxelPosition.x / chunkSize;
        }
        else if (chunkPosition.x == chunksCount.x - 1)
        {
            xRatio = 1 - (float)voxelPosition.x / chunkSize;
        }

        if (chunkPosition.y == 0)
        {
            yRatio = (float)voxelPosition.y / chunkSize;
        }
        else if (chunkPosition.y == chunksCount.y - 1)
        {
            yRatio = 1 - (float)voxelPosition.y / chunkSize;
        }

        return xRatio * yRatio;
    }
}