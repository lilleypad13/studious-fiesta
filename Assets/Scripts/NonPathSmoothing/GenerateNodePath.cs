using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateNodePath : MonoBehaviour
{
    /*
     * Finds the path of nodes between two nodes by cycling through the parent hierarchy of the endNode 
     * until it reaches the startNode.
     */
    public Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
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

    public Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
}
