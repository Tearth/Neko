using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviourSingleton<TerrainManager>
{
    public int ChunkSize;
    public Vector2 ChunksCount;
    public int SpaceHeight;
    public int BaseTerrainHeight;
    public int MaxTerrainHeight;
    public float PerlinNoiseScale;
    public GameObject Chunk;
    public GameObject Voxel;

    private bool[,,] _terrain;

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
        _terrain = _terrainGenerator.Generate((int)TotalVoxelsCount.x, (int)TotalVoxelsCount.y, SpaceHeight, BaseTerrainHeight, MaxTerrainHeight, PerlinNoiseScale);
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

        for (var x = 0; x < ChunksCount.x; x++)
        {
            for (var y = 0; y < ChunksCount.y; y++)
            {
                BuildChunk(x, y);
            }
        }
    }

    private void BuildChunk(int chunkX, int chunkY)
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
                    var voxelData = _terrain[x + terrainOffsetX, y + terrainOffsetY, z];

                    if (voxelData)
                    {
                        var visibilityData = IsVoxelVisible(terrainOffsetX + x, terrainOffsetY + y, z, _terrain);

                        _voxelBuilder.Position = new Vector3(x, z, y);
                        _voxelBuilder.TopFace = visibilityData.Top;
                        _voxelBuilder.FrontFace = visibilityData.Front;
                        _voxelBuilder.BackFace = visibilityData.Back;
                        _voxelBuilder.RightFace = visibilityData.Right;
                        _voxelBuilder.LeftFace = visibilityData.Left;
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

        var meshCollider = chunk.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    private VoxelVisibilityData IsVoxelVisible(int x, int y, int z, bool[,,] terrain)
    {
        return new VoxelVisibilityData
        {
            Top   =                                     !terrain[x, y, z + 1],
            Front = y == (int)TotalVoxelsCount.y - 1 || !terrain[x, y + 1, z],
            Back  = y == 0                           || !terrain[x, y - 1, z],
            Right = x == (int)TotalVoxelsCount.x - 1 || !terrain[x + 1, y, z],
            Left  = x == 0                           || !terrain[x - 1, y, z]
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
                            if (Vector3.Distance(voxelToCheck, fixedVoxelPosition) <= 2 && _terrain[(int)voxelToCheck.x, (int)voxelToCheck.z, (int)voxelToCheck.y])
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
        _terrain[(int)voxelPosition.x, (int)voxelPosition.z, (int)voxelPosition.y] = false;

        //var voxelAfterExplosion = Instantiate(Voxel, voxelPosition, Quaternion.identity);
        //voxelAfterExplosion.transform.GetChild(0).GetComponent<Rigidbody>().AddExplosionForce(500, hitPoint, 20, 20);
    }
}