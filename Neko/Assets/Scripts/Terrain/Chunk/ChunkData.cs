using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    private VoxelBuilder _voxelBuilder;
    private TerrainGenerator _terrainGenerator;

    private VoxelData[,,] _voxels;
    private bool _modified;

    public ChunkData()
    {
        _voxelBuilder = new VoxelBuilder();
        _terrainGenerator = new TerrainGenerator();
    }

    public void Generate(int xPos, int yPos, int height, int baseHeight, int maxHeight, int size, float noiseScale)
    {
        _voxels = _terrainGenerator.Generate(xPos, yPos, height, size, baseHeight, maxHeight, noiseScale);
        UpdateMesh(height, size);
    }

    public bool Update(int height, int size)
    {
        if (_modified)
        {
            UpdateMesh(height, size);
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

    private void UpdateMesh(int height, int size)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                for (var z = 0; z < height; z++)
                {
                    var voxelData = _voxels[x, y, z];

                    if (voxelData != null)
                    {
                        voxelData.Visibility = GetVisibilityData(x, y, z, size);

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

        var meshFilter = GetComponent<MeshFilter>();
        var meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uv.ToArray();
        meshFilter.mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private VoxelVisibilityData GetVisibilityData(int x, int y, int z, int size)
    {
        return new VoxelVisibilityData
        {
            Top   =                  _voxels[x, y, z + 1] == null,
            Front = y == size - 1 || _voxels[x, y + 1, z] == null,
            Back  = y == 0        || _voxels[x, y - 1, z] == null,
            Right = x == size - 1 || _voxels[x + 1, y, z] == null,
            Left  = x == 0        || _voxels[x - 1, y, z] == null
        };
    }
}