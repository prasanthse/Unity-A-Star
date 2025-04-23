using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid grid;

    private void Start()
    {
        grid = new Grid(4, 2, 10f, Vector3.zero);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(Utils.GetMouseWorldPosition(), 1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(Utils.GetMouseWorldPosition()));
        }
    }
}