using UnityEngine;

public class TerrainGenerator
{
    public int[,] Generate(int width, int height, float noiseScale, float maxHeight)
    {
        var heightMap = new int[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var perlinX = x * noiseScale / width;
                var perlinY = y * noiseScale / height;

                heightMap[x, y] = (int)(Mathf.PerlinNoise(perlinX, perlinY) * maxHeight);
            }
        }

        return heightMap;
    }
}