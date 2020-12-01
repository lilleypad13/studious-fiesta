using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum DataResolutionComparison
{
    Same,
    MoreDataThanPathing,
    LessDataThanPathing
}

[RequireComponent(typeof(CSVReader))]
public class ModifyDataForPathingNodes : MonoBehaviour
{
    private static ModifyDataForPathingNodes instance;
    private ModifyDataForPathingNodes()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static ModifyDataForPathingNodes Instance
    {
        get
        {
            if (instance == null)
            {
                new ModifyDataForPathingNodes();
            }
            return instance;
        }
    }

    private CSVReader csvReader;
    [SerializeField]private AGrid aGrid;

    private float distanceRatio = 0.0f; // D
    private int ratioDimension; // D*
    private DataResolutionComparison resolutionComparisonCase;
    public DataResolutionComparison ResolutionComparisonCase { get => resolutionComparisonCase; }

    private int[,] rectangularData;

    private bool IndicesAreWithinTheBounds(int[,] array, int xIndex, int yIndex)
    {
        return xIndex < array.GetLength(1) && yIndex < array.GetLength(0);
    }

    public void DetermineCaseForCreatingDataArray(float distanceBetweenDataPoints)
    {
        distanceRatio = aGrid.NodeDiameter / distanceBetweenDataPoints;

        if (distanceRatio == 1.0f)
        {
            resolutionComparisonCase = DataResolutionComparison.Same;
            ratioDimension = (int)distanceRatio;
        }
        else if (distanceRatio > 1.0f)
        {
            resolutionComparisonCase = DataResolutionComparison.MoreDataThanPathing;
            ratioDimension = (int)distanceRatio;
        }
        else if (distanceRatio < 1.0f)
        {
            resolutionComparisonCase = DataResolutionComparison.LessDataThanPathing;
            ratioDimension = (int)(1.0f / distanceRatio);
        }

        Debug.Log($"Data Grid Assignment Class Data: \n" +
            $"Case Determined: {resolutionComparisonCase.ToString()} \n" +
            $"Distance Ratio: {distanceRatio} \n" +
            $"Ratio Dimension: {ratioDimension}");
    }

    public void CheckToModifyData(int[,] dataArray)
    {
        if (resolutionComparisonCase == DataResolutionComparison.LessDataThanPathing)
            rectangularData = ModifyData(dataArray);
        else
            rectangularData = dataArray;
    }

    private int[,] ModifyData(int[,] dataArray)
    {
        int modifiedDataWidth = ratioDimension * dataArray.GetLength(0);
        int modifiedDataLength = ratioDimension * dataArray.GetLength(1);
        int[,] modifiedData = new int[modifiedDataWidth, modifiedDataLength];
        Debug.Log("Modified data to fit with pathing node grid. \n" +
            $"Modified Array has dimensions: {modifiedDataWidth}, {modifiedDataLength}.");

        int currentIndexX = 0;
        int currentIndexZ = 0;

        for (int i = 0; i < dataArray.GetLength(0); i++)
        {
            for (int j = 0; j < dataArray.GetLength(1); j++)
            {
                for (int a = 0; a < ratioDimension; a++)
                {
                    for (int b = 0; b < ratioDimension; b++)
                    {
                        currentIndexX = i * ratioDimension + a;
                        currentIndexZ = j * ratioDimension + b;
                        modifiedData[currentIndexX, currentIndexZ] = dataArray[i, j];
                    }
                }
            }
        }

        DebugListOut2DArray(modifiedData, "Here is the data array after being modified: ");

        return modifiedData;
    }

    public void CheckToAssignValue(Node node)
    {
        int valueToAssign = 0;

        if (resolutionComparisonCase == DataResolutionComparison.MoreDataThanPathing)
            valueToAssign = AverageSummationDataAssignment(node);
        else
            valueToAssign = DirectDataAssignment(node);

        node.Connectivity = valueToAssign;
    }

    private int DirectDataAssignment(Node node)
    {
        int xIndex = node.gridX;
        int yIndex = node.gridY;

        if (IndicesAreWithinTheBounds(rectangularData, xIndex, yIndex))
            return rectangularData[xIndex, yIndex];
        else
        {
            Debug.LogWarning($"Node[{xIndex}, {yIndex}]: File reader attempted to assign value to a node whose index is outside of the file's data bounds.");
            return 0;
        }
    }

    private int AverageSummationDataAssignment(Node node)
    {
        int average = 0;
        int summation = 0;

        int xIndex = node.gridX;
        int yIndex = node.gridY;

        int minXIndex = ratioDimension * xIndex;
        int minYIndex = ratioDimension * yIndex;
        int maxXIndex = ratioDimension * (xIndex + 1) - 1;
        int maxYIndex = ratioDimension * (yIndex + 1) - 1;

        if (IndicesAreWithinTheBounds(rectangularData, maxXIndex, maxYIndex))
        {
            for (int i = minXIndex; i < maxXIndex; i++)
            {
                for (int j = minYIndex; j < maxYIndex; j++)
                {
                    summation += rectangularData[i, j];
                }
            }

            average = (int)(summation / (ratioDimension * ratioDimension));
        }
        else
        {
            Debug.LogWarning($"Node[{xIndex}, {yIndex}]: File reader attempted to assign value to a node whose index is outside of the file's data bounds.");
            average = 0;
        }

        return average;
    }

    #region Debugging

    private void DebugListOut2DArray(int[,] array, string debugMessageStart)
    {
        string message = debugMessageStart + "\n";

        for (int i = 0; i < array.GetLength(0); i++)
        {
            message += $"Row {i}: ";

            for (int j = 0; j < array.GetLength(1); j++)
            {
                message += $" {array[j, i]}";
            }

            message += "\n";
        }

        Debug.Log(message);
        ExportDataToDebugLog(message);
    }

    private void ExportDataToDebugLog(string debugMessage)
    {
        string filePath = $"Assets/Resources/DebugLogMoreDATATEST.txt";
        StreamWriter file = new StreamWriter(filePath, true);

        file.Write(debugMessage);
        file.Close();
    }
    #endregion
}
