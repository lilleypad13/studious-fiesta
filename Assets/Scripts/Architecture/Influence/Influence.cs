﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Influence : MonoBehaviour
{
    #region Variables

    private Renderer renderer;
    protected Renderer Renderer { get => renderer; set => renderer = value; }

    public int xInfluence = 5;
    public int zInfluence = 5;

    [SerializeField]private int influenceAmount;
    public int InfluenceAmount
    {
        get => influenceAmount;
        set
        {
            if (value > MathArchCost.Instance.MIN_ARCHVALUE)
            {
                if (value < MathArchCost.Instance.MAX_ARCHVALUE)
                    influenceAmount = value;
                else
                    influenceAmount = MathArchCost.Instance.MAX_ARCHVALUE;
            }
            else
                influenceAmount = MathArchCost.Instance.MIN_ARCHVALUE;
        }
    }

    public int xRange { get => xInfluence / 2; }
    public int zRange { get => zInfluence / 2; }

    private Vector3 influenceOriginPosition;
    public Vector3 InfluenceOriginPosition { 
        get => influenceOriginPosition;
        set => influenceOriginPosition = value;
    }

    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        // Added for cases where value was set in the Inspector and should still be checked
        if (InfluenceAmount < MathArchCost.Instance.MAX_ARCHVALUE)
        {
            if (InfluenceAmount < MathArchCost.Instance.MIN_ARCHVALUE)
                InfluenceAmount = MathArchCost.Instance.MIN_ARCHVALUE;
        }
        else
            InfluenceAmount = MathArchCost.Instance.MAX_ARCHVALUE;

        Renderer = this.gameObject.GetComponent<Renderer>();
        InfluenceOriginPosition = DetermineInfluenceOriginPosition();

        Debug.Log($"{this.gameObject.name} has influence origin: {influenceOriginPosition}.");
    }

    protected Vector3 DetermineInfluenceOriginPosition()
    {
        return renderer.bounds.center;
    }

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
    private int xIndex = 0;
    private int zIndex = 0;
    private int rows = 0;
    private int columns = 0;

    private bool isOnPositiveGrid => xIndex >= 0 && zIndex >= 0;
    private bool isWithinGridBounds => xIndex < columns && zIndex < rows;


    public bool WithinNodeGridBounds(Node[,] grid, int _xIndex, int _zIndex)
    {
        rows = grid.GetLength(0);
        columns = grid.GetLength(1);

        xIndex = _xIndex;
        zIndex = _zIndex;

        if (isOnPositiveGrid)
            return isWithinGridBounds;
        else
            return false;
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
