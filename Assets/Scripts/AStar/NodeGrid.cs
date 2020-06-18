using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid
{
    public Node[,] Grid { get { return grid; } }
    private Node[,] grid;

    public Vector2 GridWorldSize { get { return gridWorldSize; } }
    private Vector2 gridWorldSize;

    public int GridSizeX { get { return gridSizeX; } }
    private int gridSizeX;

    public int GridSizeY { get { return gridSizeY; } }
    private int gridSizeY;

    // Constructor
    public NodeGrid(Node[,] _grid, Vector2 _gridWorldSize, int _gridSizeX, int _gridSizeY)
    {
        grid = _grid;
        gridWorldSize = _gridWorldSize;
        gridSizeX = _gridSizeX;
        gridSizeY = _gridSizeY;
    }
}
