﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataRecorder : MonoBehaviour
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

    [SerializeField] private string pathDataFileName = "PathData_";
    private string fixedFilePath = "Assets/Resources/PathData/";

    public DropdownPathHistory dropdownPathHistory;

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
     * Allows access to a particular PathData within pathRecords by index.
     */
    public PathData GetPathRecord(int index)
    {
        return pathRecords[index];
    }

    /*
     * Adds the finished currentPath to the pathRecords after filling all the data in a path
     */
    public void AddCompletedPathRecord()
    {
        pathRecords.Add(currentPath);
        dropdownPathHistory.AddNewPathToDropdown();
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
        currentPath.SetAgentAffinitiesString(agent);
    }

    public void SetCurrentPathPath(Node[] path)
    {
        currentPath.SetNodePath(path);
        currentPath.SetNodePathString(path);
    }

    
    public void SetCurrentPathSpawn(Vector3 spawnPosition)
    {
        //currentPath.SetSpawnPoint(spawnPoint);
        currentPath.SetSpawnPointName(spawnPosition);
    }

    public void SetCurrentPathDistance(float distance)
    {
        currentPath.PathTotalDistance = distance.ToString();
    }

    public void SetCurrentPathTotalArchCost(int cost)
    {
        currentPath.PathTotalArchitecturalCost = cost.ToString();
    }

    public void SetCurrentPathTotalCost(int cost)
    {
        currentPath.PathTotalCost = cost.ToString();
    }


    /*
     * Last step of organizing the string formats of all the path history data before 
     * sending it off to be exported in a text file of some kind.
     */
    public void OutputPathData()
    {
        int identifier = 0;

        foreach (PathData path in pathRecords)
        {
            // Path of file and file name creation
            string agentPathInformation = "";
            string individualFileName = pathDataFileName + identifier.ToString();

            // File content
            agentPathInformation += "Agent ID: ," + identifier + "\n" +
                path.GetFullStringData();

            // Create file and add content
            Debug.Log(agentPathInformation);
            ExportDataToCSV(agentPathInformation, individualFileName);

            identifier++;
        }
    }

    /*
     * Responsible for outputting all the path history information to a text file format
     * useable outside of Unity.
     */
    public void ExportDataToCSV(string fileContent, string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName + ".txt";
        Debug.Log("File path output: " + filePath);

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, fileContent);
        }
    }
}
