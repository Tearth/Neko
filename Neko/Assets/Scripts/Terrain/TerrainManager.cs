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

    public TerrainManager()
    {
        _terrainGenerator = new TerrainGenerator();
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
        var squareCount = 0;

        for (var x = 0; x < TerrainWidth; x++)
        {
            for (var y = 0; y < TerrainHeight; y++)
            {
                vertices.Add(new Vector3(x, terrain[x, y], y));
                vertices.Add(new Vector3(x + 1, terrain[x, y], y));
                vertices.Add(new Vector3(x + 1, terrain[x, y], y - 1));
                vertices.Add(new Vector3(x, terrain[x, y], y - 1));

                triangles.Add(squareCount * 4);
                triangles.Add(squareCount * 4 + 1);
                triangles.Add(squareCount * 4 + 3);
                triangles.Add(squareCount * 4 + 1);
                triangles.Add(squareCount * 4 + 2);
                triangles.Add(squareCount * 4 + 3);

                squareCount++;
            }
        }

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void Update()
    {

    }
}