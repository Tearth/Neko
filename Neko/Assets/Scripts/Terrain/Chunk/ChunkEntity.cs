using System.Collections.Generic;
using UnityEngine;

public class ChunkEntity : MonoBehaviour
{
    private VoxelBuilder _voxelBuilder;
    private TerrainGenerator _terrainGenerator;
    private TerrainManager _terrainManager;

    private VoxelData[,,] _voxels;
    private bool _modified;

    private Vector2Int _position;

    public void Awake()
    {
        _voxelBuilder = new VoxelBuilder();
        _terrainGenerator = new TerrainGenerator();
        _terrainManager = TerrainManager.Instance;
    }

    public void GenerateTerrainData(Vector2Int position)
    {
        _position = position;

        _voxels = _terrainGenerator.Generate(_position);
        _modified = true;
    }

    public bool UpdateMesh(ChunkEntity[] neighbourChunks)
    {
        if (_modified)
        {
            UpdateMeshData(neighbourChunks);
            _modified = false;

            return true;
        }

        return false;
    }

    public VoxelData GetVoxel(Vector3Int coordinates)
    {
        return _voxels[coordinates.x, coordinates.y, coordinates.z];
    }

    public bool RemoveVoxel(Vector3Int coordinates)
    {
        if (_voxels[coordinates.x, coordinates.y, coordinates.z] != null)
        {
            _voxels[coordinates.x, coordinates.y, coordinates.z] = null;
            _modified = true;

            return true;
        }

        return false;
    }

    private void UpdateMeshData(ChunkEntity[] neighbourChunks)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        for (var x = 0; x < _terrainManager.ChunkSize; x++)
        {
            for (var y = 0; y < _terrainManager.ChunkSize; y++)
            {
                for (var z = 0; z < _terrainManager.SpaceHeight; z++)
                {
                    var voxelData = _voxels[x, y, z];

                    if (voxelData != null)
                    {
                        voxelData.Visibility = GetVisibilityData(x, y, z);

                        if (x == 0 && neighbourChunks[0] != null)
                        {
                            voxelData.Visibility.Left = neighbourChunks[0]._voxels[_terrainManager.ChunkSize - 1, y, z] == null;
                        }

                        if (x == _terrainManager.ChunkSize - 1 && neighbourChunks[1] != null)
                        {
                            voxelData.Visibility.Right = neighbourChunks[1]._voxels[0, y, z] == null;
                        }

                        if (y == 0 && neighbourChunks[2] != null)
                        {
                            voxelData.Visibility.Back = neighbourChunks[2]._voxels[x, _terrainManager.ChunkSize - 1, z] == null;
                        }

                        if (y == _terrainManager.ChunkSize - 1 && neighbourChunks[3] != null)
                        {
                            voxelData.Visibility.Front = neighbourChunks[3]._voxels[x, 0, z] == null;
                        }

                        _voxelBuilder.Position = new Vector3(x, z, y);
                        _voxelBuilder.Visibility = voxelData.Visibility;
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
            Top    = z == _terrainManager.SpaceHeight - 1 || _voxels[x, y, z + 1] == null,
            Bottom = z > 0 && _voxels[x, y, z - 1] == null,

            Front  = y < _terrainManager.ChunkSize - 1    && _voxels[x, y + 1, z] == null,
            Back   = y > 0 && _voxels[x, y - 1, z] == null,

            Right  = x < _terrainManager.ChunkSize - 1    && _voxels[x + 1, y, z] == null,
            Left   = x > 0 && _voxels[x - 1, y, z] == null
        };
    }
}