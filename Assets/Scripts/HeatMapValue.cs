using System.Collections.Generic;
using UnityEngine;

public enum GRID_HIGHLIGHT
{
    pink,
    yellow,
    green,
    black
}

public class HeatMapValue<T>
{
    private Grid<T> grid;
    private Mesh mesh;

    public HeatMapValue(MeshFilter meshFilter)
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    internal void SetGrid(Grid<T> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();
    }

    private void UpdateHeatMapVisual()
    {
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        //for (int x = 0; x < grid.GetWidth(); x++)
        //{
        //    for (int y = 0; y < grid.GetHeight(); y++)
        //    {
        //        CreateMeshArrays(x, y, ref verticesList, ref uvsList, ref trianglesList);
        //    }
        //}

        for (int y = 0; y < grid.GetHeight(); y++)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                CreateMeshArrays(x, y, ref verticesList, ref uvsList, ref trianglesList);
            }    
        }

        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvsList.ToArray();
        mesh.triangles = trianglesList.ToArray();
    }

    private void CreateMeshArrays(int x, int y, ref List<Vector3> verticesList, ref List<Vector2> uvsList, ref List<int> trianglesList)
    {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        float CELL_SIZE = grid.GetCellSize();

        Vector3 originPosition = new Vector3(x, y, 0) * CELL_SIZE + grid.GetOriginPosition();

        vertices[0] = originPosition;
        vertices[1] = vertices[0] + Vector3.up * CELL_SIZE;
        vertices[2] = vertices[1] + Vector3.right * CELL_SIZE;
        vertices[3] = vertices[0] + Vector3.right * CELL_SIZE;

        int totalVertices = verticesList.Count;

        triangles[0] = totalVertices + 0;
        triangles[1] = totalVertices + 1;
        triangles[2] = totalVertices + 2;

        triangles[3] = totalVertices + 0;
        triangles[4] = totalVertices + 2;
        triangles[5] = totalVertices + 3;

        verticesList.AddRange(vertices);
        uvsList.AddRange(GetHighlightColor(0, uvs, GRID_HIGHLIGHT.pink));
        trianglesList.AddRange(triangles);
    }

    internal void HighlightMesh(int x, int y, GRID_HIGHLIGHT color)
    {
        int uvStartIndex = x + y * grid.GetWidth();

        mesh.uv = GetHighlightColor(uvStartIndex, mesh.uv, color);
    }

    private Vector2[] GetHighlightColor(int uvStartIndex, Vector2[] existingUVArray, GRID_HIGHLIGHT color)
    {
        Vector2[] uv = existingUVArray;

        uvStartIndex *= 4;

        switch (color)
        {
            case GRID_HIGHLIGHT.yellow:
                uv[uvStartIndex] = new Vector2(0, 0);
                uv[uvStartIndex + 1] = new Vector2(0, 0.5f);
                uv[uvStartIndex + 2] = new Vector2(0.5f, 0.5f);
                uv[uvStartIndex + 3] = new Vector2(0.5f, 0);

                break;

            case GRID_HIGHLIGHT.green:
                uv[uvStartIndex] = new Vector2(0.5f, 0.5f);
                uv[uvStartIndex + 1] = new Vector2(0.5f, 1f);
                uv[uvStartIndex + 2] = new Vector2(1f, 1f);
                uv[uvStartIndex + 3] = new Vector2(1f, 0.5f);

                break;

            case GRID_HIGHLIGHT.black:
                uv[uvStartIndex] = new Vector2(0.5f, 0);
                uv[uvStartIndex + 1] = new Vector2(0.5f, 0.5f);
                uv[uvStartIndex + 2] = new Vector2(1f, 0.5f);
                uv[uvStartIndex + 3] = new Vector2(1f, 0);

                break;

            default:
            case GRID_HIGHLIGHT.pink:
                uv[uvStartIndex] = new Vector2(0, 0.5f);
                uv[uvStartIndex + 1] = new Vector2(0, 1f);
                uv[uvStartIndex + 2] = new Vector2(0.5f, 1f);
                uv[uvStartIndex + 3] = new Vector2(0.5f, 0.5f);

                break;
        }

        return uv;
    }
}