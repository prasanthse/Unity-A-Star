using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private bool gridTest = false;

    private Grid<GenericGridTestObject> grid;
    private PathFinding pathFinding;

    private const float CELL_SIZE = 10f;

    private Vector3 originPosition;

    private void Start()
    {
        originPosition = GetGridOriginPosition();

        CalculateGridSize(out int width, out int height);

        // Test the grid
        if (gridTest)
        {
            grid = new Grid<GenericGridTestObject>(width, height, CELL_SIZE, originPosition, (Grid<GenericGridTestObject> grid, int x, int y) => new GenericGridTestObject(grid, x, y));
        }
        // Test path finding
        else
        {
            pathFinding = new PathFinding(width, height, CELL_SIZE, originPosition);
        }
    }

    private void Update()
    {
        // Test the grid
        if (gridTest)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenericGridTestObject genericGridTestObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

                if (genericGridTestObject != null)
                {
                    genericGridTestObject.AssignRandomValue();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GenericGridTestObject genericGridTestObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

                if (genericGridTestObject != null)
                {
                    Debug.Log(genericGridTestObject.ToString());
                }
                else
                {
                    Debug.Log("Unknown area");
                }
            }
        }
        // Test path finding
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                pathFinding.GetGrid().GetXY(Utils.GetMouseWorldPosition(), out int x, out int y);

                List<PathNode> path = pathFinding.FindPath(0, 0, x, y);

                if (path != null)
                {
                    for(int i = 0; i < path.Count - 1; i++)
                    {
                        Vector3 startPos = new Vector3(path[i].GetX(), path[i].GetY()) * CELL_SIZE + Vector3.one * CELL_SIZE * 0.5f;
                        Vector3 endPos = new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * CELL_SIZE + Vector3.one * CELL_SIZE * 0.5f;

                        Vector3 sP = startPos + originPosition;
                        Vector3 eP = endPos + originPosition;

                        Debug.DrawLine(sP, eP, Color.red, 100f);
                    }
                }
            }
        }
    }

    private Vector3 GetGridOriginPosition()
    {
        float cornerYPos = Camera.main.orthographicSize;
        float cornerXPos = (Camera.main.orthographicSize / Screen.height) * Screen.width;

        return new Vector3(-cornerXPos, -cornerYPos);
    }

    private void CalculateGridSize(out int width, out int height)
    {
        float screenWidth = 2 * (Camera.main.orthographicSize / Screen.height) * Screen.width;
        width = Mathf.RoundToInt(screenWidth / CELL_SIZE);

        float screenHeight = 2 * Camera.main.orthographicSize;
        height = Mathf.RoundToInt(screenHeight / CELL_SIZE);
    }
}

public class GenericGridTestObject
{
    private const int MIN = 0;
    private const int MAX = 100;

    private int value;

    private Grid<GenericGridTestObject> grid;
    private int x;
    private int y;

    public GenericGridTestObject(Grid<GenericGridTestObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AssignRandomValue()
    {
        value = Random.Range(MIN, MAX);
        grid.TriggerGridObjectChanged(x, y, value.ToString());
    }

    public override string ToString()
    {
        return value.ToString();
    }
}