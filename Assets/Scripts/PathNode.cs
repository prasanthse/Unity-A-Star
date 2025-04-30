public class PathNode
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    internal int gCost;
    internal int hCost;
    internal int fCost;

    internal PathNode cameFromNode;

    internal bool isInActiveState;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        isInActiveState = true;
    }

    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    internal int GetX()
    {
        return x;
    }

    internal int GetY()
    {
        return y;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }
}