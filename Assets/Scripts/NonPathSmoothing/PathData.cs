using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData
{
    // Contains useful data to track for every path of each agent
    private UnitSimple agentData;
    private Node[] nodePath;
    private Transform spawnPoint;

    public void SetAgentData(UnitSimple agent)
    {
        agentData = agent;
    }

    public UnitSimple GetAgentData()
    {
        return agentData;
    }

    public void SetNodePath(Node[] path)
    {
        nodePath = path;
    }

    public Node[] GetNodePath()
    {
        return nodePath;
    }

    public void SetSpawnPoint(Transform location)
    {
        spawnPoint = location;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }

}
