﻿using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    private VoxelBuilder _voxelBuilder;
    private TerrainGenerator _terrainGenerator;

    private VoxelData[,,] _voxels;
    private bool _modified;

    private Vector2Int _position;
    private int _height;
    private int _baseHeight;
    private int _maxHeight;
    private int _size;
    private float _noiseScale;

    public ChunkData()
    {
        _voxelBuilder = new VoxelBuilder();
        _terrainGenerator = new TerrainGenerator();
    }

    public void Generate(Vector2Int position, int height, int baseHeight, int maxHeight, int size, float noiseScale)
    {
        _position = position;
        _height = height;
        _baseHeight = baseHeight;
        _maxHeight = maxHeight;
        _size = size;
        _noiseScale = noiseScale;

        _voxels = _terrainGenerator.Generate(_position, _height, _size, _baseHeight, _maxHeight, _noiseScale);
        UpdateMesh();
    }

    public bool Update()
    {
        if (_modified)
        {
            UpdateMesh();
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

    private void UpdateMesh()
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
                        voxelData.Visibility = GetVisibilityData(x, y, z);

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

    private VoxelVisibilityData GetVisibilityData(int x, int y, int z)
    {
        return new VoxelVisibilityData
        {
            Top   =                  _voxels[x, y, z + 1] == null,
            Front = y < _size - 1 && _voxels[x, y + 1, z] == null,
            Back  = y > 0         && _voxels[x, y - 1, z] == null,
            Right = x < _size - 1 && _voxels[x + 1, y, z] == null,
            Left  = x > 0         && _voxels[x - 1, y, z] == null
        };
    }
}