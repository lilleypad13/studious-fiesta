using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData
{
    // Contains useful data to track for every path of each agent
    private UnitSimple agentData;
    private Node[] nodePath;
    private Transform spawnPoint;

    private string spawnPointName;
    private string agentAffinities = "";
    private string pathArchitecturalType;
    private string pathString = "Node Path: "; // Initial segment of the string that lists all the nodes within a path

    // Methods for Agent Data
    public void SetAgentData(UnitSimple agent)
    {
        agentData = agent;
    }

    public UnitSimple GetAgentData()
    {
        return agentData;
    }

    /*
     * Takes a UnitSimple object and uses its data to fill the agentAffinities string 
     * with information on that UnitSimple object's affinities as strings
     */
    public void SetAgentAffinitiesString(UnitSimple agent)
    {
        agentAffinities = "Window Affinity: ," + agent.Window.ToString() + "\n" +
            "Connectivity Affinity: ," + agent.Connectivity.ToString() + "\n";
    }

    // Methods for Node Paths
    public void SetNodePath(Node[] path)
    {
        nodePath = path;
    }

    public Node[] GetNodePath()
    {
        return nodePath;
    }

    /*
     * Takes a single path of nodes and converts it into a long string of 
     * all the indivdual node's coordinates within that path.
     */
    public void SetNodePathString(Node[] path)
    {
        foreach (Node node in path)
        {
            // Comma added in front to keep data properly in line
            pathString += "," + node.NodeCoordinates + "\n";
        }
    }

    // Methods for Spawn Points
    public void SetSpawnPoint(Transform location)
    {
        spawnPoint = location;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }

    /*
     * Takes a spawn point transform and returns the name of its gameobject as the spawn point's 
     * name as a string.
     */
    public void SetSpawnPointName(Transform spawnPoint)
    {
        spawnPointName = "Spawned At: ," + spawnPoint.name + "\n";
    }
    public void SetSpawnPointName(Vector3 spawnPosition)
    {
        spawnPointName = "Spawned At: ," + spawnPosition + "\n";
    }

    /*
     * Used to keep track of what architectural element type was used in this path
     */
    public void SetPathArchitecturalType(string pathType)
    {
        pathArchitecturalType = pathType;
    }

    public string GetPathArchitecturalType()
    {
        return pathArchitecturalType;
    }

    public string GetFullStringData()
    {
        string fullStringData = "";

        fullStringData += spawnPointName +
            agentAffinities + 
            pathString;

        return fullStringData;
    }

}
