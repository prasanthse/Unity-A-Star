using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                debugTextArray[x, y] = Utils.CreateWorldText(gridArray[x,  y].ToString(), localPosition: GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, fontSize: 20, color: Color.black, textAnchor: TextAnchor.MiddleCenter);

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.black, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.black, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.black, 100f);
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.black, 100f);
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

    private void SetValue(int x, int y, int value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    internal void SetValue(Vector3 worldPosition, int value)
    {
        GetXY(worldPosition, out int x, out int y);

        SetValue(x, y, value);
    }

    private int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    internal int GetValue(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);

        return GetValue(x, y);
    }
}