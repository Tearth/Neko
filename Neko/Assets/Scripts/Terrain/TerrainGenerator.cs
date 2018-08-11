using UnityEngine;

public class TerrainGenerator
{
    public bool[,,] Generate(int length, int width, int height, int baseHeight, int maxHeight, float noiseScale)
    {
        var heightMap = new bool[length, width, height];

        for (var x = 0; x < length; x++)
        {
            for (var y = 0; y < width; y++)
            {
                var perlinX = x * noiseScale / length;
                var perlinY = y * noiseScale / width;
                var topVoxelHeight = (int)(Mathf.PerlinNoise(perlinX, perlinY) * maxHeight);

                for (var z = 0; z <= topVoxelHeight; z++)
                {
                    heightMap[x, y, z] = true;
                }
            }
        }

        return heightMap;
    }
}