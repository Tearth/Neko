using System.Collections.Generic;
using UnityEngine;

public class VoxelBuilder
{
    public Vector3 Position { get; set; }
    public bool TopFace { get; set; }
    public bool FrontFace { get; set; }
    public bool BackFace { get; set; }
    public bool RightFace { get; set; }
    public bool LeftFace { get; set; }
    public Texture2D Texture { get; set; }

    public VoxelBuilder()
    {
        Reset();
    }

    public void GenerateAndAddToLists(List<Vector3> vertices, List<int> triangles, List<float> uv)
    {
        var squareCount = vertices.Count / 4;
        if (TopFace)
        {
            GenerateTopFace(vertices, triangles, uv, squareCount);
            squareCount++;
        }

        if (FrontFace)
        {
            GenerateFrontFace(vertices, triangles, uv, squareCount);
            squareCount++;
        }

        if (BackFace)
        {
            GenerateBackFace(vertices, triangles, uv, squareCount);
            squareCount++;
        }

        if (RightFace)
        {
            GenerateRightFace(vertices, triangles, uv, squareCount);
            squareCount++;
        }

        if (LeftFace)
        {
            GenerateLeftFace(vertices, triangles, uv, squareCount);
            squareCount++;
        }
    }

    public void Reset()
    {
        Position = Vector3.zero;
        TopFace = false;
        FrontFace = false;
        BackFace = false;
        RightFace = false;
        LeftFace = false;
        Texture = null;
    }

    private void GenerateTopFace(List<Vector3> vertices, List<int> triangles, List<float> uv, int squareNumber)
    {
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z + 1));
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z + 1));

        AddTrianglesForSquare(triangles, squareNumber);
    }

    private void GenerateFrontFace(List<Vector3> vertices, List<int> triangles, List<float> uv, int squareNumber)
    {
        vertices.Add(new Vector3(Position.x + 1,    Position.y,         Position.z + 1));
        vertices.Add(new Vector3(Position.x,        Position.y,         Position.z + 1));
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z + 1));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z + 1));

        AddTrianglesForSquare(triangles, squareNumber);
    }

    private void GenerateBackFace(List<Vector3> vertices, List<int> triangles, List<float> uv, int squareNumber)
    {
        vertices.Add(new Vector3(Position.x,        Position.y,         Position.z));
        vertices.Add(new Vector3(Position.x + 1,    Position.y,         Position.z));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z));
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z));

        AddTrianglesForSquare(triangles, squareNumber);
    }

    private void GenerateRightFace(List<Vector3> vertices, List<int> triangles, List<float> uv, int squareNumber)
    {
        vertices.Add(new Vector3(Position.x + 1,    Position.y,         Position.z));
        vertices.Add(new Vector3(Position.x + 1,    Position.y,         Position.z + 1));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z + 1));
        vertices.Add(new Vector3(Position.x + 1,    Position.y + 1,     Position.z));

        AddTrianglesForSquare(triangles, squareNumber);
    }

    private void GenerateLeftFace(List<Vector3> vertices, List<int> triangles, List<float> uv, int squareNumber)
    {
        vertices.Add(new Vector3(Position.x,        Position.y,         Position.z + 1));
        vertices.Add(new Vector3(Position.x,        Position.y,         Position.z));
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z));
        vertices.Add(new Vector3(Position.x,        Position.y + 1,     Position.z + 1));

        AddTrianglesForSquare(triangles, squareNumber);
    }

    private void AddTrianglesForSquare(List<int> triangles, int squareNumber)
    {
        triangles.Add(squareNumber * 4);
        triangles.Add(squareNumber * 4 + 3);
        triangles.Add(squareNumber * 4 + 2);
        triangles.Add(squareNumber * 4 + 2);
        triangles.Add(squareNumber * 4 + 1);
        triangles.Add(squareNumber * 4);
    }
}
