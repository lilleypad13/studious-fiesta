using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Node: IHeapItem<Node>
{
    // General fields
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;
    public Node parent;

    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }
    private int heapIndex;


    // Base cost parameters
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public Dictionary<string, ArchitecturalElement> architecturalElementTypes = new Dictionary<string, ArchitecturalElement>();

    // Debugging Variables
    public string NodeCoordinates { get { return gridX + " , " + gridY; } } // Helpful for identifying a specific node during debugging

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
        PopulateDictionaryFromGlobal();
    }

    public Node(Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = true;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = 0;
        PopulateDictionaryFromGlobal();
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
        return -compare;
    }

    private void PopulateDictionaryFromGlobal()
    {
        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            ArchitecturalElement element = new ArchitecturalElement(container.Key);
            architecturalElementTypes.Add(container.Key, element);
        }
    }

    /*
     * Debugging method to check architectural impacts on grid
     */
    public void ArchitecturalOutput()
    {
        string debugMessage = $"Node: {NodeCoordinates} architectural values: \n";

        foreach (KeyValuePair<string, ArchitecturalElement> element in architecturalElementTypes)
        {
            debugMessage += $"{element.Key} has value {element.Value.ArchitecturalValue}\n";
        }

        Debug.Log(debugMessage);
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
