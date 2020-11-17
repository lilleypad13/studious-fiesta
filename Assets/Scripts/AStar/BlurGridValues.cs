using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurGridValues : MonoBehaviour
{
    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MinValue;

    /*
     * Creates a new grid of nodes from an original grid by blending penalty values throughout the map together 
     * so there are less discrete sections for areas with different penalty values.
     */
    public Node[,] BlurPenaltyMap(Node[,] grid, int blurSize)
    {
        Node[,] nodeGrid = grid;

        int gridRows = nodeGrid.GetLength(0);
        int gridColumns = nodeGrid.GetLength(1);

        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = blurSize;

        // Creates two new grids of equal size to the original grid to hold the updated blurred values.
        // The horizontal pass uses values from the original grid to fill its own first, 
        // then the vertical pass uses the values from the horizontal grid to fill itself after.
        int[,] penaltiesHorizontalPass = new int[gridRows, gridColumns];
        int[,] penaltiesVerticalPass = new int[gridRows, gridColumns];

        // All of the Mathf.Clamp methods used are to effectively reproduce values at the boundaries of the grid 
        // to keep anything from going out of the grid bounds upon calculation.
        for (int y = 0; y < gridColumns; y++)
        {
            // Fills the first element in the row based on the kernel size
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += nodeGrid[sampleX, y].movementPenalty;
            }

            // Fills the rest of the elements after the first
            // As this process moves throughout the grid, it starts with the last value calculated in the grid and subtracts 
            // the leftmost elements from that value (that were dropped from the kernel) and adds the new rightmost elements 
            // (newly added to the kernel upon advancing) to obtain the value for the current grid location.
            for (int x = 1; x < gridRows; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridRows);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridRows - 1);

                // Generates the next value by simply subtracting the value from the single element 
                // removed from the kernel and adding the value from the newly added element
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y]
                    - nodeGrid[removeIndex, y].movementPenalty
                    + nodeGrid[addIndex, y].movementPenalty;
            }
        }

        // Same process, but reverse x and y for vertical process
        for (int x = 0; x < gridRows; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            nodeGrid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridColumns; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridColumns);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridColumns - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1]
                    - penaltiesHorizontalPass[x, removeIndex]
                    + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                nodeGrid[x, y].movementPenalty = blurredPenalty;

                // Solely for visualization purposes
                if (blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;
                if (blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }

        return nodeGrid;
    }

    
}
