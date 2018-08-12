using UnityEngine;

public class TerrainGenerator
{
    public VoxelData[,,] Generate(int length, int width, int height, int baseHeight, int maxHeight, float noiseScale)
    {
        var heightMap = new VoxelData[length, width, height];

        for (var x = 0; x < length; x++)
        {
            for (var y = 0; y < width; y++)
            {
                var perlinX = x * noiseScale / length;
                var perlinY = y * noiseScale / width;
                var topVoxelHeight = (int)(Mathf.PerlinNoise(perlinX, perlinY) * maxHeight);

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