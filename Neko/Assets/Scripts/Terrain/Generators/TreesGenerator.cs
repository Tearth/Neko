using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TreesGenerator
{
    private TerrainManager _terrainManager;
    private Random _random;

    public TreesGenerator()
    {
        _terrainManager = TerrainManager.Instance;
        _random = new Random();
    }

    public List<TreeEntity> Generate()
    {
        var trees = new List<TreeEntity>();

        for (int i = 0; i < _terrainManager.TreeCount; i++)
        {
            var randomPosition = GetRandomPosition();
            var chunk = _terrainManager.GetChunkByVoxelCoordinates(new Vector3Int(randomPosition.x, 0, randomPosition.y));
            var highestPoint = chunk.GetHighestPoint(new Vector2Int(randomPosition.x % 16, randomPosition.y % 16));

            var tree = Inst
        }

        return trees;
    }

    private Vector2Int GetRandomPosition()
    {
        var totalVoxelCount = _terrainManager.TotalVoxelCount;
        return new Vector2Int(_random.Next(0, totalVoxelCount.x), _random.Next(0, totalVoxelCount.y));
    }
}