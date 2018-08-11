using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public int TerrainWidth;
    public int TerrainHeight;
    public float NoiseScale;
    public int MaxHeight;
    public GameObject VoxelPrefab;

    private TerrainGenerator _terrainGenerator;
    private VoxelBuilder _voxelBuilder;

    public TerrainManager()
    {
        _terrainGenerator = new TerrainGenerator();
        _voxelBuilder = new VoxelBuilder();
    }

    private void Start()
    {
        var terrain = _terrainGenerator.Generate(TerrainWidth, TerrainHeight, NoiseScale, MaxHeight);

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        //for (var x = 0; x < TerrainWidth; x++)
        //{
        //    for (var y = 0; y < TerrainHeight; y++)
        //    {
                _voxelBuilder.Position = new Vector3(0, 0, 0);
                _voxelBuilder.TopFace = true;
        _voxelBuilder.FrontFace = true;
        _voxelBuilder.BackFace = true;
        _voxelBuilder.RightFace = true;
        _voxelBuilder.LeftFace = true;
        _voxelBuilder.GenerateAndAddToLists(vertices, triangles, null);
        //    }
        //}

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void Update()
    {

    }
}