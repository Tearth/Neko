using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkEntity : MonoBehaviour
{
    public int PreferredTreeCount;
    public int MinimalTreeHeight;
    public int MinimalDistanceBetweenTrees;

    public bool Modified;
    public ChunkEntity[] NeighbourChunks;
    public List<GameObject> TreePrefabs;

    private VoxelBuilder _voxelBuilder;
    private TerrainGenerator _terrainGenerator;

    private VoxelData[,,] _voxels;
    private List<TreeEntity> _trees;

    private Vector2Int _position;
    private Vector2Int _chunksCount;
    private int _height;
    private int _baseHeight;
    private int _maxHeight;
    private int _size;
    private float _noiseScale;

    public ChunkEntity()
    {
        _voxelBuilder = new VoxelBuilder();
        _terrainGenerator = new TerrainGenerator();
    }

    public void GenerateTerrainData(Vector2Int position, Vector2Int chunksCount, int height, int baseHeight, int maxHeight, int size, float noiseScale)
    {
        _position = position;
        _chunksCount = chunksCount;
        _height = height;
        _baseHeight = baseHeight;
        _maxHeight = maxHeight;
        _size = size;
        _noiseScale = noiseScale;

        _voxels = _terrainGenerator.Generate(_position, _chunksCount, _height, _size, _baseHeight, _maxHeight, _noiseScale);
        Modified = true;
    }

    public void GenerateTrees(List<TreeEntity> globalTreeList)
    {
        _trees = new List<TreeEntity>();

        for (int i = 0; i < PreferredTreeCount; i++)
        {
            var position = new Vector2Int(Random.Range(0, _size), Random.Range(0, _size));
            var treeChunkCoordinates = new Vector3Int(position.x, GetHighestPoint(position), position.y);
            var treeWorldCoordinates = treeChunkCoordinates + new Vector3Int(_size * _position.x, 0, _size * _position.y);

            if (treeChunkCoordinates.y > MinimalTreeHeight && !globalTreeList.Any(p => Vector3.Distance(p.transform.position, treeWorldCoordinates) < MinimalDistanceBetweenTrees))
            {
                var treePrefab = TreePrefabs[Random.Range(0, TreePrefabs.Count)];

                var gameObject = Instantiate(treePrefab, treeWorldCoordinates, Quaternion.identity, transform);
                var treeEntity = gameObject.GetComponent<TreeEntity>();

                _trees.Add(treeEntity);
                globalTreeList.Add(treeEntity);
            }
        }
    }

    public bool UpdateMesh()
    {
        if (Modified)
        {
            UpdateMeshData();
            Modified = false;

            return true;
        }

        return false;
    }

    public VoxelData GetVoxel(Vector3Int coordinates)
    {
        return _voxels[coordinates.x, coordinates.y, coordinates.z];
    }

    public int GetHighestPoint(Vector2Int coordinates)
    {
        for (var z = 0; z < _height; z++)
        {
            if (_voxels[coordinates.x, coordinates.y, z] == null)
            {
                return z;
            }
        }

        return -1;
    }

    public bool AddVoxel(Vector3Int coordinates)
    {
        if (_voxels[coordinates.x, coordinates.y, coordinates.z] == null)
        {
            _voxels[coordinates.x, coordinates.y, coordinates.z] = new VoxelData
            {
                Visibility = GetVisibilityData(coordinates.x, coordinates.y, coordinates.z),
                Type = VoxelType.Dirt
            };

            Modified = true;
            UpdateNeighbourVoxels(coordinates, false);

            if (coordinates.x == 0 || coordinates.x == _size - 1 || coordinates.y == 0 || coordinates.y == _size - 1)
            {
                UpdateNeighbourChunks(coordinates);
            }

            return true;
        }

        return false;
    }

    public bool RemoveVoxel(Vector3Int coordinates)
    {
        if (_voxels[coordinates.x, coordinates.y, coordinates.z] != null)
        {
            _voxels[coordinates.x, coordinates.y, coordinates.z] = null;
            Modified = true;

            UpdateNeighbourVoxels(coordinates, true);

            if (coordinates.x == 0 || coordinates.x == _size - 1 || coordinates.y == 0 || coordinates.y == _size - 1)
            {
                UpdateNeighbourChunks(coordinates);
            }

            return true;
        }

        return false;
    }

    private void UpdateNeighbourVoxels(Vector3Int coordinates, bool voxelRemoved)
    {
        if (coordinates.x > 0 && _voxels[coordinates.x - 1, coordinates.y, coordinates.z] != null)
        {
            _voxels[coordinates.x - 1, coordinates.y, coordinates.z].Visibility.Right = voxelRemoved;
        }

        if (coordinates.x < _size - 1 && _voxels[coordinates.x + 1, coordinates.y, coordinates.z] != null)
        {
            _voxels[coordinates.x + 1, coordinates.y, coordinates.z].Visibility.Left = voxelRemoved;
        }

        if (coordinates.y > 0 && _voxels[coordinates.x, coordinates.y - 1, coordinates.z] != null)
        {
            _voxels[coordinates.x, coordinates.y - 1, coordinates.z].Visibility.Front = voxelRemoved;
        }

        if (coordinates.y < _size - 1 && _voxels[coordinates.x, coordinates.y + 1, coordinates.z] != null)
        {
            _voxels[coordinates.x, coordinates.y + 1, coordinates.z].Visibility.Back = voxelRemoved;
        }

        if (coordinates.z > 0 && _voxels[coordinates.x, coordinates.y, coordinates.z - 1] != null)
        {
            _voxels[coordinates.x, coordinates.y, coordinates.z - 1].Visibility.Top = voxelRemoved;
        }

        if (coordinates.z < _size - 1 && _voxels[coordinates.x, coordinates.y, coordinates.z + 1] != null)
        {
            _voxels[coordinates.x, coordinates.y, coordinates.z + 1].Visibility.Bottom = voxelRemoved;
        }
    }

    private void UpdateNeighbourChunks(Vector3Int coordinates)
    {
        if (NeighbourChunks[0] != null && coordinates.x == 0)
        {
            NeighbourChunks[0].Modified = true;
        }

        if (NeighbourChunks[1] != null && coordinates.x == _size - 1)
        {
            NeighbourChunks[1].Modified = true;
        }

        if (NeighbourChunks[2] != null && coordinates.y == 0)
        {
            NeighbourChunks[2].Modified = true;
        }

        if (NeighbourChunks[3] != null && coordinates.y == _size - 1)
        {
            NeighbourChunks[3].Modified = true;
        }
    }

    private void UpdateMeshData()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        for (var x = 0; x < _size; x++)
        {
            for (var y = 0; y < _size; y++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var voxelData = _voxels[x, y, z];

                    if (voxelData != null)
                    {
                        if (voxelData.Visibility == null)
                        {
                            voxelData.Visibility = GetVisibilityData(x, y, z);
                        }

                        if (x == 0 && NeighbourChunks[0] != null)
                        {
                            voxelData.Visibility.Left = NeighbourChunks[0]._voxels[_size - 1, y, z] == null;
                        }

                        if (x == _size - 1 && NeighbourChunks[1] != null)
                        {
                            voxelData.Visibility.Right = NeighbourChunks[1]._voxels[0, y, z] == null;
                        }

                        if (y == 0 && NeighbourChunks[2] != null)
                        {
                            voxelData.Visibility.Back = NeighbourChunks[2]._voxels[x, _size - 1, z] == null;
                        }

                        if (y == _size - 1 && NeighbourChunks[3] != null)
                        {
                            voxelData.Visibility.Front = NeighbourChunks[3]._voxels[x, 0, z] == null;
                        }

                        _voxelBuilder.Position = new Vector3(x, z, y);
                        _voxelBuilder.TopFace = voxelData.Visibility.Top;
                        _voxelBuilder.BottomFace = voxelData.Visibility.Bottom;
                        _voxelBuilder.FrontFace = voxelData.Visibility.Front;
                        _voxelBuilder.BackFace = voxelData.Visibility.Back;
                        _voxelBuilder.RightFace = voxelData.Visibility.Right;
                        _voxelBuilder.LeftFace = voxelData.Visibility.Left;
                        _voxelBuilder.TextureType = voxelData.Type;

                        _voxelBuilder.GenerateAndAddToLists(vertices, triangles, uv);
                    }
                }
            }
        }

        var meshFilter = GetComponent<MeshFilter>();
        var meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uv.ToArray();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private VoxelVisibilityData GetVisibilityData(int x, int y, int z)
    {
        return new VoxelVisibilityData
        {
            Top    = z == _height - 1 || _voxels[x, y, z + 1] == null,
            Bottom = z > 0            && _voxels[x, y, z - 1] == null,

            Front  = y < _size - 1    && _voxels[x, y + 1, z] == null,
            Back   = y > 0            && _voxels[x, y - 1, z] == null,

            Right  = x < _size - 1    && _voxels[x + 1, y, z] == null,
            Left   = x > 0            && _voxels[x - 1, y, z] == null
        };
    }
}