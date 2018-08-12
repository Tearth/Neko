using System;
using UnityEngine;

public class TerrainGenerator
{
    public VoxelData[,,] Generate(int xPos, int yPos, int height, int size, int baseHeight, int maxHeight, float noiseScale)
    {
        var heightMap = new VoxelData[size, size, height];

        var perlinXPos = xPos * noiseScale;
        var perlinYPos = yPos * noiseScale;
        var step = 1f * noiseScale / size;

        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                var perlinX = perlinXPos + x * step;
                var perlinY = perlinYPos + y * step;
                var topVoxelHeight = (int)(Mathf.Clamp(Mathf.PerlinNoise(perlinX, perlinY), 0, 1) * maxHeight);

                var topVoxelData = new VoxelData();
                topVoxelData.Type = VoxelType.Dirt;

                heightMap[x, y, topVoxelHeight] = topVoxelData;

                for (var z = 0; z < topVoxelHeight; z++)
                {
                    var voxelData = new VoxelData();
                    voxelData.Type = VoxelType.Stone;

                    heightMap[x, y, z] = voxelData;
                }
            }
        }

        return heightMap;
    }
}