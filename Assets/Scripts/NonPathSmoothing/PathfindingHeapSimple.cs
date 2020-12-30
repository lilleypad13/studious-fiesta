﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.UI;

public enum ArchitecturePathingData
{
    window, 
    connectivity
}

[RequireComponent(typeof(GenerateNodePath))]
public class PathfindingHeapSimple : MonoBehaviour
{
    [Tooltip("In Game UI Also")]
    [SerializeField] private ArchitecturePathingData typeUsedForPathing;

    [SerializeField] private bool isFindingNearestWalkableNode = false;
    [SerializeField] private int walkableNodeSearchIterations = 1;
    private PathRequestManagerSimple requestManager;
    private AGridRuntime aGridRuntime;
    private GenerateNodePath generateNodePath;
    private const int MAX_AFFINITY = 100;

    // Architecture Cost Rates
    private int windowRate = 0;
    private int connectivityRate = 0;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManagerSimple>();
        generateNodePath = GetComponent<GenerateNodePath>();
        aGridRuntime = AGridRuntime.Instance;
    }

    public void StartFindPath(Vector3 startPosition, Vector3 targetPosition, UnitSimple agent)
    {
        StartCoroutine(FindPath(startPosition, targetPosition, agent));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition, UnitSimple agent)
    {
        // Stopwatch just used to time the processing
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Start of true pathfinding
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = FindAppropriateNodeFromWorldPoint(startPosition, isFindingNearestWalkableNode);
        Node targetNode = FindAppropriateNodeFromWorldPoint(targetPosition, isFindingNearestWalkableNode);
        UnityEngine.Debug.Log($"PathfindingHeap for {agent.name}: \n" +
            $" Start walkable: {startNode.walkable}\n" + 
            $" Target Walkability: {targetNode.walkable}");


        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(aGridRuntime.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                // The target has been found, so exit out of the loop
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    //RetracePath(startNode, targetNode);
                    break;
                }

                CalcAgentArchitectureRates(agent); // Required before cycling through all the nodes

                foreach (Node neighbor in aGridRuntime.GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    // Where extra variables are factored into the gCost

                    // Where all architectural cost is totaled (including additional default architectural cost for each 
                    // architectural element type)
                    int architecturalCost = 0;

                    switch (typeUsedForPathing)
                    {
                        case ArchitecturePathingData.window:
                            architecturalCost = windowRate * currentNode.Window + MathArchCost.Instance.ARCHCOST_DEFAULT;
                            break;
                        case ArchitecturePathingData.connectivity:
                            architecturalCost = connectivityRate * MathArchCost.Instance.NormalizeConnectivity(currentNode.Connectivity) + MathArchCost.Instance.ARCHCOST_DEFAULT;
                            break;
                        default:
                            architecturalCost = MathArchCost.Instance.ARCHCOST_DEFAULT;
                            break;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor)
                        + neighbor.movementPenalty + architecturalCost;

                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                            openSet.UpdateItem(neighbor);
                    }
                }
            }
        }
        
        yield return null;
        if (pathSuccess)
        {
            DataRecorder.Instance.SetCurrentPathAgent(agent);
            DataRecorder.Instance.SetCurrentArchitecturalType(typeUsedForPathing.ToString());
            waypoints = generateNodePath.RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }


    private Node FindAppropriateNodeFromWorldPoint(Vector3 worldPosition, bool isFindingNearestNode)
    {
        if (isFindingNearestWalkableNode)
            return aGridRuntime.FindNearestWalkableNodeFromWorldPoint(worldPosition, walkableNodeSearchIterations);
        else
            return aGridRuntime.NodeFromWorldPoint(worldPosition);
    }

    
    private void CalcAgentArchitectureRates(UnitSimple agent)
    {
        windowRate = MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.Window);
        connectivityRate = MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.Connectivity);
    }


    /*
     * Returns an approximated and scaled integer distance value between two nodes.
     * Orthogonal moves have a normalized distance of 1, where diagonal moves are then relatively 
     * the sqrt(2), which is approximately 1.4. These values are then multiplied by 10 to give their 
     * respective values of 10 and 14 for ease of use and readability.
     */
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        else
            return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    /*
     * Allows setting of architecture type used for pathing by the enum's index
     */
    public void SetArchitecturalType(int index)
    {
        typeUsedForPathing = (ArchitecturePathingData)index;
    }
}
