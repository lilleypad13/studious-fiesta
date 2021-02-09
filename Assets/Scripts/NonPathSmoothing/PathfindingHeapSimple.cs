using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(GenerateNodePath))]
public class PathfindingHeapSimple : MonoBehaviour
{
    [SerializeField] private bool isFindingNearestWalkableNode = true;
    [SerializeField] private int walkableNodeSearchIterations = 3;
    private PathRequestManagerSimple requestManager;
    private GenerateNodePath generateNodePath;


    private void Awake()
    {
        requestManager = GetComponent<PathRequestManagerSimple>();
        generateNodePath = GetComponent<GenerateNodePath>();
        
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
            Heap<Node> openSet = new Heap<Node>(AGridRuntime.Instance.MaxSize);
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

                foreach (Node neighbor in AGridRuntime.Instance.GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    // Factor extra costs into the gCost
                    // Architectural cost of all elements
                    int architecturalCost = CalculateArchitecturalCost(agent, neighbor);

                    // Total cost
                    int newMovementCostToNeighbor = currentNode.gCost + 
                        GetDistance(currentNode, neighbor) + 
                        neighbor.movementPenalty + 
                        architecturalCost;

                    //UnityEngine.Debug.Log($"Costs of Node {currentNode.NodeCoordinates}:\n" +
                    //    $"G Cost: {currentNode.gCost}" +
                    //    $"Arch Cost: {architecturalCost}");

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
            DataRecorder.Instance.SetCurrentPathDistance(CalculateTotalPathDistance(startNode, targetNode));
            DataRecorder.Instance.SetCurrentPathTotalArchCost(CalculateTotalPathArchCost(startNode, targetNode, agent));
            DataRecorder.Instance.SetCurrentPathTotalCost(CalculateTotalPathCost(startNode, targetNode));
            waypoints = generateNodePath.RetracePath(startNode, targetNode);


            UnityEngine.Debug.Log($"Pathing Total Costs: \n " +
            $"Total Path Architectural Cost was: {CalculateTotalPathArchCost(startNode, targetNode, agent)}\n" +
            $"Total Path Cost was: {CalculateTotalPathCost(startNode, targetNode)}");
        }

        
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private static int CalculateArchitecturalCost(UnitSimple agent, Node currentNode)
    {
        int architecturalCost = 0;

        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            architecturalCost += MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.affinityTypes[container.Key].AffinityValue) *
                MathArchCost.Instance.NormailzeArchitecturalValue(currentNode.architecturalElementTypes[container.Key]) +
                MathArchCost.Instance.ARCHCOST_DEFAULT;

            //UnityEngine.Debug.Log($"Calculated Arch Cost from:\n" +
            //    $"Affinity: {agent.affinityTypes[container.Key].AffinityValue}" +
            //    $"Rate: {MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.affinityTypes[container.Key].AffinityValue)}" +
            //    $"Architectural Value: {currentNode.architecturalElementTypes[container.Key].ArchitecturalValue}" +
            //    $"Normalized Arch Value: {MathArchCost.Instance.NormailzeArchitecturalValue(currentNode.architecturalElementTypes[container.Key])}" +
            //    $"Partial Arch Cost: {architecturalCost}");
        }

        return architecturalCost;
    }

    private int CalculateTotalPathArchCost(Node startNode, Node endNode, UnitSimple agent)
    {
        int cost = 0;
        Node currentNode = endNode;
        cost = CalculateArchitecturalCost(agent, currentNode);

        while (currentNode != startNode)
        {
            currentNode = currentNode.parent;
            cost += CalculateArchitecturalCost(agent, currentNode);
        }

        return cost;
    }

    private int CalculateTotalPathCost(Node startNode, Node endNode)
    {
        int cost = 0;
        Node currentNode = endNode;
        cost = currentNode.gCost;

        while (currentNode != startNode)
        {
            currentNode = currentNode.parent;
            cost += currentNode.gCost;
        }

        return cost;
    }

    private float CalculateTotalPathDistance(Node startNode, Node endNode)
    {
        float distance = 0.0f;
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            distance += Vector3.Distance(currentNode.worldPosition, currentNode.parent.worldPosition);
            currentNode = currentNode.parent;
        }

        return distance;
    }

    private Node FindAppropriateNodeFromWorldPoint(Vector3 worldPosition, bool isFindingNearestNode)
    {
        if (isFindingNearestWalkableNode)
            return AGridRuntime.Instance.FindNearestWalkableNodeFromWorldPoint(worldPosition, walkableNodeSearchIterations);
        else
            return AGridRuntime.Instance.NodeFromWorldPoint(worldPosition);
    }


    /*
     * Returns an approximated and scaled integer distance value between two nodes.
     * Orthogonal moves have a normalized distance of 1, where diagonal moves are then relatively 
     * the sqrt(2), which is approximately 1.4. These values are then multiplied by 10 to give their 
     * respective values of 10 and 14 for ease of use and readability.
     */
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        else
            return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
