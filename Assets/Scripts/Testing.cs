using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private bool gridTest = false;

    private Grid<GenericGridTestObject> grid;
    private PathFinding pathFinding;

    private const float CELL_SIZE = 10f;

    private Vector3 originPosition;

    [SerializeField] private MeshFilter meshFilter;

    private HeatMapValue<GenericGridTestObject> gridTestHeatMap;
    private HeatMapValue<PathNode> pathNodeHeatMap;

    private bool isShowPathHighlights = false;

    private void Awake()
    {
        if (gridTest)
        {
            gridTestHeatMap = new HeatMapValue<GenericGridTestObject>(meshFilter);
        }
        else
        {
            pathNodeHeatMap = new HeatMapValue<PathNode>(meshFilter);
        }
    }

    private void Start()
    {
        originPosition = GetGridOriginPosition();

        CalculateGridSize(out int width, out int height);

        // Test the grid
        if (gridTest)
        {
            grid = new Grid<GenericGridTestObject>(width, height, CELL_SIZE, originPosition, (Grid<GenericGridTestObject> grid, int x, int y) => new GenericGridTestObject(grid, x, y));
            gridTestHeatMap.SetGrid(grid);

            gridTestHeatMap.HighlightMesh(0, 0, GRID_HIGHLIGHT.yellow);
        }
        // Test path finding
        else
        {
            pathFinding = new PathFinding(width, height, CELL_SIZE, originPosition);
            pathNodeHeatMap.SetGrid(pathFinding.GetGrid());

            pathNodeHeatMap.HighlightMesh(0, 0, GRID_HIGHLIGHT.yellow);
        }

        PlayerManager.Instance.SetPosition(originPosition + Vector3.one * CELL_SIZE * 0.5f);
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

                    if (isShowPathHighlights)
                    {
                        gridTestHeatMap.HighlightMesh(genericGridTestObject.GetX(), genericGridTestObject.GetY(), GRID_HIGHLIGHT.green);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GenericGridTestObject genericGridTestObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

                if (genericGridTestObject != null)
                {
                    Debug.Log(genericGridTestObject.ToString());

                    gridTestHeatMap.HighlightMesh(genericGridTestObject.GetX(), genericGridTestObject.GetY(), GRID_HIGHLIGHT.black);
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
                pathFinding.GetGrid().GetXY(PlayerManager.Instance.transform.localPosition, out int startX, out int startY);
                pathFinding.GetGrid().GetXY(Utils.GetMouseWorldPosition(), out int x, out int y);

                List<PathNode> path = pathFinding.FindPath(startX, startY, x, y);

                List<Vector3[]> playerMovePos = new List<Vector3[]>();

                if (path != null)
                {
                    for(int i = 0; i < path.Count - 1; i++)
                    {
                        Vector3 startPos = new Vector3(path[i].GetX(), path[i].GetY()) * CELL_SIZE + Vector3.one * CELL_SIZE * 0.5f;
                        Vector3 endPos = new Vector3(path[i + 1].GetX(), path[i + 1].GetY()) * CELL_SIZE + Vector3.one * CELL_SIZE * 0.5f;

                        Vector3 sP = startPos + originPosition;
                        Vector3 eP = endPos + originPosition;

                        if (isShowPathHighlights)
                        {
                            pathNodeHeatMap.HighlightMesh(path[i + 1].GetX(), path[i + 1].GetY(), GRID_HIGHLIGHT.green);
                        }

                        playerMovePos.Add(new Vector3[2] { sP, eP });
                    }

                    pathNodeHeatMap.HighlightMesh(path[path.Count - 1].GetX(), path[path.Count - 1].GetY(), GRID_HIGHLIGHT.green);

                    PlayerManager.Instance.Move(playerMovePos);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                PathNode path = pathFinding.GetGrid().GetGridObject(Utils.GetMouseWorldPosition());

                if (path != null)
                {
                    pathNodeHeatMap.HighlightMesh(path.GetX(), path.GetY(), GRID_HIGHLIGHT.black);

                    path.isInActiveState = false;
                }
                else
                {
                    Debug.Log("Unknown area");
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

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}