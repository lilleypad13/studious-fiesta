using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.UI;

public enum ArchitecturePathingData
{
    window, 
    connectivity
}

public class PathfindingHeapSimple : MonoBehaviour
{
    [Tooltip("In Game UI Also")]
    [SerializeField] private ArchitecturePathingData typeUsedForPathing;

    private PathRequestManagerSimple requestManager;
    private AGridRuntime aGridRuntime;
    private const int MAX_AFFINITY = 100;

    // Architecture Cost Rates
    private int windowRate = 0;
    private int connectivityRate = 0;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManagerSimple>();
        aGridRuntime = GetComponent<AGridRuntime>();
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

        Node startNode = aGridRuntime.NodeFromWorldPoint(startPosition);
        Node targetNode = aGridRuntime.NodeFromWorldPoint(targetPosition);

        if (startNode.walkable && targetNode.walkable)
        {
            UnityEngine.Debug.Log("Start and Target walkable");
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
                            architecturalCost = connectivityRate * MathArchCost.Instance.normalizeConnectivity(currentNode.Connectivity) + MathArchCost.Instance.ARCHCOST_DEFAULT;
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
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    /*
     * Sets all the architecutral rates for a given agent
     */
    private void CalcAgentArchitectureRates(UnitSimple agent)
    {
        windowRate = MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.Window);
        connectivityRate = MathArchCost.Instance.CalculateCostPerArchFromAffinity(agent.Connectivity);
    }

    /*
     * Finds the path of nodes between two nodes by cycling through the parent hierarchy of the endNode 
     * until it reaches the startNode.
     */
    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            //currentNode.OutputNodeCosts();
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Node[] pathArray = path.ToArray();
        DataRecorder.Instance.SetCurrentPathPath(pathArray);
        //Vector3[] waypoints = SimplifyPath(path);
        List<Vector3> waypointsList = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            waypointsList.Add(path[i].worldPosition);
        }
        Vector3[] waypoints = waypointsList.ToArray();

        Array.Reverse(waypoints);
        return waypoints;
        //path.Reverse();
        //aGrid.path = path;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
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
