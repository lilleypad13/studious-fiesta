using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRecorder
{
    private static DataRecorder instance;
    private DataRecorder()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static DataRecorder Instance
    {
        get
        {
            if (instance == null)
            {
                new DataRecorder();
            }
            return instance;
        }
    }

    private static List<PathData> pathRecords = new List<PathData>();
    private static PathData currentPath;

    /*
     * Add a pathData element to the records
     */
    public void AddPathRecord(PathData pathData)
    {
        pathRecords.Add(pathData);
    }

    /*
     * Sets the current path to determine which PathData to currently be adding data to
     */
    public void SetCurrentPath(PathData path)
    {
        currentPath = path;
    }

    /*
     * Adds the finished currentPath to the pathRecords after filling all the data in a path
     */
    public void AddCompletedPathRecord()
    {
        pathRecords.Add(currentPath);
    }

    /*
     * Initializes a new PathData object as the starting point to fill with PathData information
     */
    public void GenerateNewPathData()
    {
        currentPath = new PathData();
        Debug.Log("Path created in recorder.");
    }

    public void SetCurrentPathAgent(UnitSimple agent)
    {
        currentPath.SetAgentData(agent);
    }

    public void SetCurrentPathPath(Node[] path)
    {
        currentPath.SetNodePath(path);
    }

    public void SetCurrentPathSpawn(Transform spawnPoint)
    {
        currentPath.SetSpawnPoint(spawnPoint);
    }

    public void OutputPathData()
    {
        int identifier = 0;

        foreach (PathData path in pathRecords)
        {
            string pathInformation = "";
            foreach (Node node in path.GetNodePath())
            {
                pathInformation += node.NodeCoordinates + "\n";
            }

            Debug.Log("Agent ID: " + identifier + "\n" +
                "Spawned At: " + path.GetSpawnPoint().name + "\n" +
                "Agent's Window Affinity: " + path.GetAgentData().Window + "\n" +
                "Agent's Connectivity Affinity: " + path.GetAgentData().Connectivity + "\n" +
                "Path List: " + pathInformation);

            identifier++;
        }
    }
}
