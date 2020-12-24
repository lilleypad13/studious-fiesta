using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class ArchitecturalPathing
{
    private string name;
    public string Name { get => name; }
    private int architecturalValue;
    public int ArchitecturalValue
    {
        get => architecturalValue;
        set
        {
            if (value > MathArchCost.Instance.MAX_ARCHVALUE)
                architecturalValue = MathArchCost.Instance.MAX_ARCHVALUE;
            else
                architecturalValue = value;
        }
    }

    public ArchitecturalPathing(string _name, int _architecturalValue)
    {
        name = _name;

        if (_architecturalValue > MathArchCost.Instance.MAX_ARCHVALUE)
            architecturalValue = MathArchCost.Instance.MAX_ARCHVALUE;
        else
            architecturalValue = _architecturalValue;
    }
}

public class Node: IHeapItem<Node>
{
    // General fields
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;
    public Node parent;
    public int heapIndex;

    // Base cost parameters
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    // Architectural Parameters
    public int Window
    {
        get { return window; }
        set
        {
            if (value > MathArchCost.Instance.MAX_ARCHVALUE)
                window = MathArchCost.Instance.MAX_ARCHVALUE;
            else
                window = value;
        }
    }
    private int window = 0;

    public int Connectivity
    {
        get { return connectivity; }
        set
        {
            connectivity = value;
        }
    }
    private int connectivity = 0;

    public Dictionary<string, ArchitecturalPathing> dictionaryOfArchitecturalPathingTypes = new Dictionary<string, ArchitecturalPathing>();

    // Debugging Variables
    public string NodeCoordinates { get { return gridX + " , " + gridY; } } // Helpful for identifying a specific node during debugging

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /*
     * Compares the fCost of this node and another node, and if they are equal, compares 
     * the hCost of the two nodes.
     * It returns the opposite of this compare value as a higher priority 
     * is represented by lower costs.
     */
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        //Debug.Log("Compare value between node " + NodeCoordinates + " and node " + nodeToCompare.NodeCoordinates + " is " + compare);
        return -compare;
    }

    /*
     * Debugging method to check architectural impacts on grid
     */
    public void ArchitecturalOutput()
    {
        Debug.Log("Node: " + NodeCoordinates + " has architectural values: " + "Window : " + Window);
    }

    /*
     * Debugging method to check the cost values of a node
     */
    public void OutputNodeCosts()
    {
        Debug.Log("Costs of node: " + NodeCoordinates + " are: \n" +
            " gCost: " + gCost + " \n" +
            " hCost: " + hCost + " \n" +
            " fCost: " + fCost);
    }
}
