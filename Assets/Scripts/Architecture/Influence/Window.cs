using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Influence
{
    protected override void Awake()
    {
        base.Awake();
    }

    /*
     * Applies influence value within a rectangular area.
     * Checks if nodes exist within range.
     */
    public override void ApplyInfluence(Node[,] grid, Node influenceOrigin)
    {
        for (int x = -xRange; x < xRange; x++)
        {
            for (int z = -zRange; z < zRange; z++)
            {
                if(WithinNodeGridBounds(grid, influenceOrigin.gridX + x, influenceOrigin.gridY + z))
                {
                    grid[influenceOrigin.gridX + x, influenceOrigin.gridY + z].Window += InfluenceAmount;
                }
            }
        }
    }
}
