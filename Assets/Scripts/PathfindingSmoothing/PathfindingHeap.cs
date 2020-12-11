using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathfindingHeap : MonoBehaviour
{
    private AGridRuntime aGridRuntime;

    private void Awake()
    {
        aGridRuntime = AGridRuntime.Instance;
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = aGridRuntime.NodeFromWorldPoint(request.pathStart);
        Node targetNode = aGridRuntime.NodeFromWorldPoint(request.pathEnd);

        if(startNode.walkable && targetNode.walkable)
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
                    //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    //RetracePath(startNode, targetNode);
                    break;
                }

                foreach (Node neighbor in aGridRuntime.GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) 
                        + neighbor.movementPenalty;
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
        
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0; // Ensures pathSuccess is set back to false if waypoints is empty
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));
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
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Is now getting the worldPosition of all the nodes in the path to pass on 
        // as the waypoints array
        //Vector3[] waypoints = SimplifyPath(path);
        List<Vector3> waypointsList = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            waypointsList.Add(path[i].worldPosition);
        }
        Vector3[] waypoints = waypointsList.ToArray();

        Array.Reverse(waypoints);
        return waypoints;
    }

    // NOT BEING USED CURRENTLY, BUT HOLD ON FOR POSSIBLE IMPLEMENTATION LATER
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
}
