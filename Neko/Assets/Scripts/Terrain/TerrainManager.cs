using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviourSingleton<TerrainManager>
{
    public int ChunkSize;
    public Vector2Int ChunksCount;
    public int SpaceHeight;
    public int BaseTerrainHeight;
    public int MaxTerrainHeight;
    public float PerlinNoiseScale;
    public GameObject ChunkPrefab;
    public GameObject VoxelPrefab;

    private ChunkEntity[,] _chunks;

    private void Start()
    {
        CreateChunks();
    }

    private void Update()
    {

    }

    public Vector3Int GetVoxelCoordinatesByHitPoint(Vector3 hitPoint)
    {
        if (hitPoint.y % 1 == 0)
        {
            hitPoint -= new Vector3(0, 0.5f, 0);
        }

        if (hitPoint.x % 1 == 0 && hitPoint.x < Camera.main.transform.position.x)
        {
            hitPoint -= new Vector3(0.5f, 0, 0);
        }

        return new Vector3Int((int)hitPoint.x, (int)hitPoint.z, (int)hitPoint.y);
    }

    public ChunkEntity GetChunkByVoxelCoordinates(Vector3Int voxelCoordinates)
    {
        var chunkX = voxelCoordinates.x / ChunkSize;
        var chunkY = voxelCoordinates.y / ChunkSize;

        if (chunkX >= 0 && chunkX < ChunksCount.x && chunkY >= 0 && chunkY < ChunksCount.y)
        {
            return _chunks[chunkX, chunkY];
        }

        return null;
    }

    public bool RemoveVoxel(Vector3Int position)
    {
        var chunk = GetChunkByVoxelCoordinates(position);
        var normalizedPosition = NormalizePosition(position);

        return chunk.RemoveVoxel(normalizedPosition);
    }

    public int UpdateChunks()
    {
        var updatedChunks = 0;
        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                var neighbourChunks = GetNeighbourChunks(new Vector2Int(x, y));
                if (_chunks[x, y].UpdateMesh(neighbourChunks))
                {
                    updatedChunks++;
                }
            }
        }

        return updatedChunks;
    }

    private void CreateChunks()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        _chunks = new ChunkEntity[ChunksCount.x, ChunksCount.y];
        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                var chunk = Instantiate(ChunkPrefab, new Vector3(x * ChunkSize, 0, y * ChunkSize), Quaternion.identity, gameObject.transform);

                var chunkScript = chunk.GetComponent<ChunkEntity>();
                chunkScript.GenerateTerrainData(new Vector2Int(x, y), ChunksCount, SpaceHeight, BaseTerrainHeight, MaxTerrainHeight, ChunkSize, PerlinNoiseScale);

                _chunks[x, y] = chunkScript;
            }
        }

        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                var neighbourChunks = GetNeighbourChunks(new Vector2Int(x, y));
                _chunks[x, y].UpdateMesh(neighbourChunks);
            }
        }
    }

    private ChunkEntity[] GetNeighbourChunks(Vector2Int chunkCoords)
    {
        var x = chunkCoords.x;
        var y = chunkCoords.y;

        return new[]
        {
            x > 0 ?                 _chunks[x - 1, y] : null,
            x < ChunksCount.x - 1 ? _chunks[x + 1, y] : null,
            y > 0 ?                 _chunks[x, y - 1] : null,
            y < ChunksCount.y - 1 ? _chunks[x, y + 1] : null
        };
    }

    private Vector3Int NormalizePosition(Vector3Int position)
    {
        return new Vector3Int(position.x % ChunkSize, position.y % ChunkSize, position.z);
    }
}