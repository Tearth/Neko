using System;
using UnityEngine;

public class TerrainGenerator
{
    public VoxelData[,,] Generate(Vector2Int position, Vector2Int chunksCount, int height, int size, int baseHeight, int maxHeight, float noiseScale)
    {
        var heightMap = new VoxelData[size, size, height];

        var perlinXPos = position.x * noiseScale;
        var perlinYPos = position.y * noiseScale;
        var step = 1f * noiseScale / size;

        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var perlinX = perlinXPos + x * step;
                var perlinY = perlinYPos + y * step;
                var edgeRatio = GetEdgeRatio(position, x, y, chunksCount, size);

                var topVoxelHeight = (int)((Mathf.Clamp(Mathf.PerlinNoise(perlinX, perlinY), 0, 1) * maxHeight) * edgeRatio) + baseHeight;

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

    private float GetEdgeRatio(Vector2Int chunkPosition, int voxelX, int voxelY, Vector2Int chunksCount, int size)
    {
        var xRatio = 1f;
        var yRatio = 1f;

        if (chunkPosition.x == 0)
        {
            xRatio = (float)voxelX / size;
        }
        else if (chunkPosition.x == chunksCount.x - 1)
        {
            xRatio = 1 - (float)voxelX / size;
        }

        if (chunkPosition.y == 0)
        {
            yRatio = (float)voxelY / size;
        }
        else if (chunkPosition.y == chunksCount.y - 1)
        {
            yRatio = 1 - (float)voxelY / size;
        }

        return xRatio * yRatio;
    }
}