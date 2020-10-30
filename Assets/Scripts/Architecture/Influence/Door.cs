﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Influence
{
    public int door = 0;

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
                    grid[influenceOrigin.gridX + x, influenceOrigin.gridY + z].Window += door;
                }
            }
        }
    }
}