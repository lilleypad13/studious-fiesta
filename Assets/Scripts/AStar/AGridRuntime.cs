using System.Collections.Generic;
using UnityEngine;

public class AGridRuntime
{
    private Node[,] grid; //2D array of nodes
    private Vector2 gridWorldSize = new Vector2();
    private int gridSizeX;
    private int gridSizeY;

    private static AGridRuntime instance = null;
    private static readonly object padlock = new object();

    public static AGridRuntime Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new AGridRuntime();
                }
            }
            return instance;
        }
    }

    /*
     * Used to pass the grid data from AGrid to here
     */
    public void SetGrid(NodeGrid nodeGrid)
    {
        grid = nodeGrid.Grid;
        gridWorldSize = nodeGrid.GridWorldSize;
        gridSizeX = nodeGrid.GridSizeX;
        gridSizeY = nodeGrid.GridSizeY;
    }

    // Returns the overall area of the grid given its dimensions
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }


    /*
     * Consistently checks if a node is within the overall grid bounds before trying to get it 
     * and set values to it.
     */
    //private int xIndex = 0;
    //private int zIndex = 0;

    public bool WithinNodeGridBounds(int xIndex, int zIndex)
    {
        int rows = grid.GetLength(0);
        int columns = grid.GetLength(1);

        bool isOnPositiveGrid = (xIndex >= 0 && zIndex >= 0);
        bool isWithinGridBounds = (xIndex < columns && zIndex < rows);

        if (isOnPositiveGrid)
            return isWithinGridBounds;
        else
            return false;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        bool foundX = false;
        bool foundY = false;

        float xDist;
        float lastXDist = Mathf.Infinity;

        float zDist;
        float lastZDist = Mathf.Infinity;

        int xCounter = 0;
        int zCounter = 0;

        int maxGridSizeCheckX = gridSizeX;
        int maxGridSizeCheckZ = gridSizeY;

        // Go through nodes starting with first node to find which node is closest on the x-axis
        while (!foundX && xCounter < maxGridSizeCheckX)
        {
            xDist = Mathf.Abs(worldPosition.x - grid[xCounter, 0].worldPosition.x);

            if(xDist >= lastXDist)
                foundX = true;
            else
            {
                lastXDist = xDist;
                xCounter++;
            }
        }

        // Follows same logic to find node closest on z-axis
        while(!foundY && zCounter < maxGridSizeCheckZ)
        {
            zDist = Mathf.Abs(worldPosition.z - grid[0, zCounter].worldPosition.z);

            if(zDist >= lastZDist)
                foundY = true;
            else
            {
                lastZDist = zDist;
                zCounter++;
            }
        }

        int lastClosestXIndex = xCounter - 1;
        int lastClosestZIndex = zCounter - 1;

        return grid[lastClosestXIndex, lastClosestZIndex];
    }

    public Node FindNearestWalkableNodeFromWorldPoint(Vector3 worldPosition, int iterations)
    {
        if (iterations == 0)
        {
            Node onlyNode = NodeFromWorldPoint(worldPosition);
            if (onlyNode.walkable)
                return onlyNode;
            else
                return null;
        }

        int iterationCounter = 1;
        int searchIndex = 0;
        Node initialNode = NodeFromWorldPoint(worldPosition);
        Node searchNode = initialNode;
        List<Node> nodesToSearch = GetNeighbors(initialNode);

        if (!searchNode.walkable)
        {
            foreach (Node node in nodesToSearch)
            {
                searchNode = node;
                if (searchNode.walkable)
                    return searchNode;
            }
        }

        while(!searchNode.walkable && 
            iterationCounter <= iterations && 
            searchIndex <= nodesToSearch.Count)
        {
            if (searchIndex < nodesToSearch.Count)
            {
                searchNode = nodesToSearch[searchIndex];
                searchIndex++;
            }
            else
            {
                searchIndex = 0;
                iterationCounter++;
                if(iterationCounter <= iterations)
                    nodesToSearch = GetNeighborsDistanceAway(initialNode, iterationCounter);
            }
        }

        if (searchNode.walkable)
            return searchNode;
        else
        {
            Debug.LogWarning("Searched for nearest walkalbe node but did not find any.");
            return initialNode;
        }
    }

    /*
     * Creates a list of all the nodes around a particular node.
     * This is checking for the 8 nodes around the node in a 3x3 grid with the node of interest at the center.
     */
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) // This is the node itself, which does not need checked
                    continue;

                // Ensures the node exists by checking if it is within the overall bounds of the grid
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    private List<Node> GetNeighborsDistanceAway(Node node, int nodesAway)
    {
        int nodesToCheck = nodesAway;
        if (nodesToCheck == 0)
        {
            List<Node> self = new List<Node>();
            self.Add(node);
            return self;
        }

        nodesToCheck = Mathf.Abs(nodesToCheck);

        List<Node> neighbors = new List<Node>();
        int xIndex = 0;
        int yIndex = 0;

        for (int x = -nodesToCheck; x <= nodesToCheck; x++)
        {
            int y = nodesToCheck;

            xIndex = node.gridX + x;
            yIndex = node.gridY + y;

            if (xIndex >= 0 && xIndex < gridSizeX &&
                yIndex >= 0 && yIndex < gridSizeY)
            {
                neighbors.Add(grid[xIndex, yIndex]);
            }
        }

        for (int x = -nodesToCheck; x <= nodesToCheck; x++)
        {
            int y = -nodesToCheck;

            xIndex = node.gridX + x;
            yIndex = node.gridY + y;

            if (xIndex >= 0 && xIndex < gridSizeX &&
                yIndex >= 0 && yIndex < gridSizeY)
            {
                neighbors.Add(grid[xIndex, yIndex]);
            }
        }

        if(nodesToCheck > 1)
        {
            nodesToCheck--;

            for (int y = -nodesToCheck; y <= nodesToCheck; y++)
            {
                int x = nodesToCheck;

                xIndex = node.gridX + x;
                yIndex = node.gridY + y;

                if (xIndex >= 0 && xIndex < gridSizeX &&
                    yIndex >= 0 && yIndex < gridSizeY)
                {
                    neighbors.Add(grid[xIndex, yIndex]);
                }
            }

            for (int y = -nodesToCheck; y <= nodesToCheck; y++)
            {
                int x = -nodesToCheck;

                xIndex = node.gridX + x;
                yIndex = node.gridY + y;

                if (xIndex >= 0 && xIndex < gridSizeX &&
                    yIndex >= 0 && yIndex < gridSizeY)
                {
                    neighbors.Add(grid[xIndex, yIndex]);
                }
            }
        }

        DebugNodeList(neighbors, $"GetNeighborsDistanceAway with nodesAway: {nodesAway}");

        return neighbors;
    }

    #region Debugging

    private void DebugNodeList(List<Node> nodes, string introDebugMessage)
    {
        string debugMessage = $"List of Node Coordinates from {introDebugMessage}: \n";

        foreach (Node node in nodes)
        {
            debugMessage += $"[{node.gridX}, {node.gridY}]\n";
        }

        Debug.Log(debugMessage);
    }

    #endregion
}
