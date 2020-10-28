using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Influence : MonoBehaviour
{
    #region Variables

    // influence range is how many 
    public int xInfluence = 5;
    public int zInfluence = 5;

    public int xRange
    {
        get { return xInfluence / 2; }
    }
    public int zRange
    {
        get { return zInfluence / 2; }
    }

    #endregion

    #region Unity Methods

    /*
     * Takes in the overall A* grid and the origin of the influence object in terms of nodes 
     * so that individual Influence type object can determine which nodes to influence and how to 
     * influence them.
     * Node origin helps localize the node editing (so Influence class knows where to start influencing 
     * and where to spread out from)
     */
    public abstract void ApplyInfluence(Node[,] grid, Node influenceOrigin);

    /*
     * Consistently checks if a node is within the overall grid bounds before trying to get it 
     * and set values to it.
     */
    public bool WithinNodeGridBounds(Node[,] grid, int xIndex, int zIndex)
    {
        bool isWithinBounds = false;

        int rows = grid.GetLength(0);
        int columns = grid.GetLength(1);

        if(xIndex >= 0 && zIndex >= 0) // Ensures valid coordinates are entered at all
        {
            if(xIndex < columns)
            {
                if (zIndex < rows)
                    isWithinBounds = true;
            }
        }

        return isWithinBounds;
    }

    /*
     * Debug to show which nodes an influence object is impacting on the grid.
     * influenceName can be manually put in to show which type of influence is being applied or the name of the object
     * applying influence.
     */
    public void DebugShowInfluencedNodes(string influenceName, int x, int z)
    {
        Debug.Log($"Applied {influenceName} to {x} , {z}.");
    }

    #endregion
}
