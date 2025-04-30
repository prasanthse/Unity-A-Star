using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    #region VARIABLES
    // Entire grid
    private Grid<PathNode> grid;

    // PathNodes that avaialble for searching
    private List<PathNode> openList;
    // Already searched PathNodes
    private List<PathNode> closeList;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    #endregion

    #region CONSTRUCTOR
    public PathFinding(int width, int height, float cellSize, Vector3 gridOriginPosition)
    {
        grid = new Grid<PathNode>(width, height, cellSize, gridOriginPosition, (Grid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));

        openList = new List<PathNode>();
        closeList = new List<PathNode>();
    }
    #endregion

    internal List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        if(startX < 0 || startY < 0 || endX < 0 || endY < 0)
        {
            Debug.Log("Invalid value passed");
            return null;
        }

        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList.Clear();
        closeList.Clear();

        openList.Add(startNode);

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            // Reached final node
            if(currentNode == endNode)
            {
                return CalculatePath(currentNode);
            }

            // Remove current node hence it already been searched
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isInActiveState)
                {
                    closeList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;

                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes in open list
        return null;
    }

    internal List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        grid.GetXY(startWorldPos, out int startX, out int startY);
        grid.GetXY(endWorldPos, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);

        if(path != null)
        {
            List<Vector3> vectorPath = new List<Vector3>();

            foreach(PathNode p in path)
            {
                vectorPath.Add(new Vector3(p.GetX(), p.GetY()) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f);
            }

            return vectorPath;
        }
        else
        {
            return null;
        }
    }

    // Calculate distance by ignoring all blockable areas
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());

        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currentNode = endNode;

        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();

        return path;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        // Left Area Nodes
        if(currentNode.GetX() - 1 >= 0)
        {
            // Left
            neighbourList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY()));

            // Left Up
            if (currentNode.GetY() + 1 < grid.GetHeight())
            {
                neighbourList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY() + 1));
            }

            // Left Down
            if (currentNode.GetY() - 1 >= 0)
            {
                neighbourList.Add(grid.GetGridObject(currentNode.GetX() - 1, currentNode.GetY() - 1));
            }     
        }

        // Right Area Nodes
        if (currentNode.GetX() + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY()));

            // Right Up
            if (currentNode.GetY() + 1 < grid.GetHeight())
            {
                neighbourList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY() + 1));
            }

            // Right Down
            if (currentNode.GetY() - 1 >= 0)
            {
                neighbourList.Add(grid.GetGridObject(currentNode.GetX() + 1, currentNode.GetY() - 1));
            }
        }

        // Top
        if (currentNode.GetY() + 1 < grid.GetHeight())
        {
            neighbourList.Add(grid.GetGridObject(currentNode.GetX(), currentNode.GetY() + 1));
        }

        // Bottom
        if (currentNode.GetY() - 1 >= 0)
        {
            neighbourList.Add(grid.GetGridObject(currentNode.GetX(), currentNode.GetY() - 1));
        }

        return neighbourList;
    }

    internal Grid<PathNode> GetGrid()
    {
        return grid;
    }
}