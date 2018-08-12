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
    public GameObject Chunk;
    public GameObject Voxel;

    private VoxelData[,,] _voxels;
    private ChunkData[,] _chunks;

    private Vector2 TotalVoxelsCount
    {
        get { return new Vector2(ChunksCount.x * ChunkSize, ChunksCount.y * ChunkSize); }
    }

    private TerrainGenerator _terrainGenerator;
    private VoxelBuilder _voxelBuilder;

    public TerrainManager()
    {
        _terrainGenerator = new TerrainGenerator();
        _voxelBuilder = new VoxelBuilder();
    }

    private void Start()
    {
        _voxels = _terrainGenerator.Generate((int)TotalVoxelsCount.x, (int)TotalVoxelsCount.y, SpaceHeight, BaseTerrainHeight, MaxTerrainHeight, PerlinNoiseScale);
        RegenerateVisibilityData();
        RegenerateTerrain();
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void RegenerateTerrain()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        _chunks = new ChunkData[ChunksCount.x, ChunksCount.y];
        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                _chunks[x, y] = BuildChunk(x, y);
            }
        }
    }

    private ChunkData BuildChunk(int chunkX, int chunkY)
    {
        var chunkData = new ChunkData();

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
                    var voxelData = _voxels[x + terrainOffsetX, y + terrainOffsetY, z];

                    if (voxelData != null)
                    {
                        _voxelBuilder.Position = new Vector3(x, z, y);
                        _voxelBuilder.TopFace = voxelData.Visibility.Top;
                        _voxelBuilder.FrontFace = voxelData.Visibility.Front;
                        _voxelBuilder.BackFace = voxelData.Visibility.Back;
                        _voxelBuilder.RightFace = voxelData.Visibility.Right;
                        _voxelBuilder.LeftFace = voxelData.Visibility.Left;
                        _voxelBuilder.TextureType = VoxelType.Dirt;
                        _voxelBuilder.GenerateAndAddToLists(vertices, triangles, uv);
                    }
                }
            }
        }

        var chunk = Instantiate(Chunk, new Vector3(chunkX * ChunkSize, 0, chunkY * ChunkSize), Quaternion.identity, gameObject.transform);
        chunkData.Mesh = chunk.GetComponent<MeshFilter>().mesh;
        chunkData.Mesh.vertices = vertices.ToArray();
        chunkData.Mesh.triangles = triangles.ToArray();
        chunkData.Mesh.uv = uv.ToArray();
        chunkData.Mesh.RecalculateNormals();

        var meshCollider = chunk.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = chunkData.Mesh;

        return chunkData;
    }

    public void RegenerateVisibilityData()
    {
        for (int x = 0; x < TotalVoxelsCount.x; x++)
        {
            for (int y = 0; y < TotalVoxelsCount.y; y++)
            {
                for (int z = 0; z < SpaceHeight; z++)
                {
                    var voxelData = _voxels[x, y, z];

                    if (voxelData != null)
                    {
                        voxelData.Visibility = GetVisibilityData(x, y, z);
                    }
                }
            }
        }
    }

    private VoxelVisibilityData GetVisibilityData(int x, int y, int z)
    {
        return new VoxelVisibilityData
        {
            Top   =                                     _voxels[x, y, z + 1] == null,
            Front = y == (int)TotalVoxelsCount.y - 1 || _voxels[x, y + 1, z] == null,
            Back  = y == 0                           || _voxels[x, y - 1, z] == null,
            Right = x == (int)TotalVoxelsCount.x - 1 || _voxels[x + 1, y, z] == null,
            Left  = x == 0                           || _voxels[x - 1, y, z] == null
        };
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var dir = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(dir, out hit))
            {
                var hitPoint = hit.point;
                if (hitPoint.y % 1 == 0) hitPoint -= new Vector3(0, 0.5f, 0);
                if (hitPoint.x % 1 == 0 && hitPoint.x < Camera.main.transform.position.x) hitPoint -= new Vector3(0.5f, 0, 0);

                var fixedVoxelPosition = new Vector3((int)hitPoint.x, (int)hitPoint.y, (int)hitPoint.z);

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        for (int z = -2; z <= 2; z++)
                        {
                            var voxelToCheck = new Vector3(x, y, z) + fixedVoxelPosition;
                            var q = Vector3.Distance(voxelToCheck, fixedVoxelPosition);
                            if (Vector3.Distance(voxelToCheck, fixedVoxelPosition) <= 2 && _voxels[(int)voxelToCheck.x, (int)voxelToCheck.z, (int)voxelToCheck.y] != null)
                            {
                                Explosion(hit.point, voxelToCheck, 1);
                            }
                        }
                    }
                }
                RegenerateTerrain();
            }
        }
    }

    private void Explosion(Vector3 hitPoint, Vector3 voxelPosition, int range)
    {
        _voxels[(int)voxelPosition.x, (int)voxelPosition.z, (int)voxelPosition.y] = null;

        //var voxelAfterExplosion = Instantiate(Voxel, voxelPosition, Quaternion.identity);
        //voxelAfterExplosion.transform.GetChild(0).GetComponent<Rigidbody>().AddExplosionForce(500, hitPoint, 20, 20);
    }
}