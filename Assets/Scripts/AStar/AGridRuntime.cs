using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGridRuntime : MonoBehaviour
{
    private Node[,] grid; //2D array of nodes
    private Vector2 gridWorldSize = new Vector2();
    private int gridSizeX;
    private int gridSizeY;

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
     * Uses a vector3 position to determine which node encompasses that position.
     * If out of bounds, will simply return the extremes of the outter edge of the grid.
     */
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        // Clamped to prevent values from going out of bounds (will never be less than 0 or greater than 1)
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Uses percent values to determine index values for node in array
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public Node TestNodeFromWorldPoint(Vector3 worldPosition)
    {
        bool foundX = false;
        bool foundY = false;

        float xDist;
        float lastXDist = Mathf.Infinity;

        float zDist;
        float lastZDist = Mathf.Infinity;

        int xCounter = 0;
        int zCounter = 0;

        int maxGridSizeCheckX = Mathf.FloorToInt(gridWorldSize.x) - 1;
        int maxGridSizeCheckZ = Mathf.FloorToInt(gridWorldSize.y) - 1;
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

        return grid[xCounter, zCounter];
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
}
