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
    private string pathTotalArchitecturalCost;

    public string PathTotalArchitecturalCost
    {
        set
        {
            pathTotalArchitecturalCost = "Total Architectural Cost of Path: " + value + "\n";
        }
    }

    private string pathTotalCost;
    public string PathTotalCost
    {
        set
        {
            pathTotalCost = "Total Overall Cost of Path: " + value + "\n";
        }
    }

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
        foreach (KeyValuePair<string, Affinity> aff in agent.affinityTypes)
        {
            agentAffinities += $"{aff.Key} Affinity: ," +
                $"{aff.Value.AffinityValue}\n";
        }
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

    public string GetFullStringData()
    {
        string fullStringData = "";

        fullStringData += spawnPointName +
            agentAffinities + 
            pathTotalArchitecturalCost + 
            pathTotalCost +
            pathString;

        return fullStringData;
    }

}
