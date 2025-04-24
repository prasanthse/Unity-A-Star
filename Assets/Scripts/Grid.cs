using System;
using UnityEngine;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    private bool showDebugView = true;

    public event EventHandler<OnGridObjectChangedEventArgs> onGridObjectChanged;
    public class OnGridObjectChangedEventArgs: EventArgs
    {
        public int x;
        public int y;
    }

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        if (showDebugView)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x, y]?.ToString(), localPosition: GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, fontSize: 20, color: Color.black, textAnchor: TextAnchor.MiddleCenter);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 newPosition = worldPosition - originPosition;

        x = Mathf.FloorToInt(newPosition.x / cellSize);
        y = Mathf.FloorToInt(newPosition.y / cellSize);
    }

    private void SetGridObject(int x, int y, TGridObject value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;

            TriggerGridObjectChanged(x, y);
        }
    }

    internal void TriggerGridObjectChanged(int x, int y, string debugViewValue = null)
    {
        if (onGridObjectChanged != null)
        {
            onGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        if(debugViewValue != null && showDebugView)
        {
            debugTextArray[x, y].text = debugViewValue;
        }
    }

    internal void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXY(worldPosition, out int x, out int y);

        SetGridObject(x, y, value);
    }

    private TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    internal TGridObject GetGridObject(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);

        return GetGridObject(x, y);
    }
}