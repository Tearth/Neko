using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public int ChunkSize;
    public Vector2 ChunksCount;
    public int SpaceHeight;
    public int BaseTerrainHeight;
    public int MaxTerrainHeight;
    public float PerlinNoiseScale;
    public GameObject Chunk;

    private TerrainGenerator _terrainGenerator;
    private VoxelBuilder _voxelBuilder;

    public TerrainManager()
    {
        _terrainGenerator = new TerrainGenerator();
        _voxelBuilder = new VoxelBuilder();
    }

    private void Start()
    {
        var totalVoxelsLength = (int)(ChunksCount.x * ChunkSize);
        var totalVoxelsWidth = (int)(ChunksCount.y * ChunkSize);

        var terrain = _terrainGenerator.Generate(totalVoxelsLength, totalVoxelsWidth, SpaceHeight, BaseTerrainHeight, MaxTerrainHeight, PerlinNoiseScale);

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                BuildChunk(x, y, terrain);
            }
        }
    }

    private void Update()
    {

    }

    private void BuildChunk(int chunkX, int chunkY, bool[,,] terrain)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        var terrainOffsetX = chunkX * ChunkSize;
        var terrainOffsetY = chunkY * ChunkSize;

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                for (int z = 0; z < SpaceHeight; z++)
                {
                    var voxelData = terrain[x + terrainOffsetX, y + terrainOffsetY, z];

                    if (voxelData)
                    {
                        _voxelBuilder.Position = new Vector3(x, z, y);
                        _voxelBuilder.TopFace = true;
                        _voxelBuilder.FrontFace = true;
                        _voxelBuilder.BackFace = true;
                        _voxelBuilder.RightFace = true;
                        _voxelBuilder.LeftFace = true;
                        _voxelBuilder.TextureType = TextureType.Dirt;
                        _voxelBuilder.GenerateAndAddToLists(vertices, triangles, uv);
                    }
                }
            }
        }

        var chunk = Instantiate(Chunk, new Vector3(chunkX * ChunkSize, 0, chunkY * ChunkSize), Quaternion.identity, gameObject.transform);
        var mesh = chunk.GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
    }
}