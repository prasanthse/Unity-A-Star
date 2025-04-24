using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid<GenericGridTestObject> grid;

    private void Start()
    {
        grid = new Grid<GenericGridTestObject>(4, 2, 10f, Vector3.zero, (Grid<GenericGridTestObject> grid, int x, int y) => new GenericGridTestObject(grid, x, y));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenericGridTestObject genericGridTestObject = grid.GetGridObject(Utils.GetMouseWorldPosition());

            if(genericGridTestObject != null)
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