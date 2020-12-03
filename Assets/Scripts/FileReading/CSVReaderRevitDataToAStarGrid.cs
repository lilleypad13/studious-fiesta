using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;

public class DataWithPosition
{
    private float xPosition;
    public float XPosition { get => xPosition; }
    private float zPosition;
    public float ZPosition { get => zPosition; }
    private int dataValue;
    public int DataValue { get => dataValue; }

    public DataWithPosition(float x, float z, int data)
    {
        xPosition = x;
        zPosition = z;
        dataValue = data;
    }

    public DataWithPosition(string x, string z, string data)
    {
        float.TryParse(x, out xPosition);
        float.TryParse(z, out zPosition);
        int.TryParse(data, out dataValue);
    }

    public DataWithPosition(string x, string z, string data, float conversionFactor)
    {
        float.TryParse(x, out xPosition);
        float.TryParse(z, out zPosition);
        int.TryParse(data, out dataValue);

        xPosition /= conversionFactor;
        zPosition /= conversionFactor;
    }

}

public class CSVReaderRevitDataToAStarGrid : MonoBehaviour
{
    [SerializeField] private AGrid aGrid;
    private ModifyDataForPathingNodes modifyData;

    // Data about the file being read
    [Header("File to Read")]
    [Tooltip("File name that can be copied from any file in the Resources folder.")]
    [SerializeField] private string rhinoInputCSVName = "DemoModel_VGACoordinates";

    [Header("File Parameters")]
    [Tooltip("EXPERIMENTAL: Generally keep at 1 (unless weird data set).")]
    [SerializeField] private float distanceBetweenDataPoints = 1.0f; // Increment value between coordinates of each data point
    [Tooltip("Value to assign empty coordinate locations not found in the file.")]
    [SerializeField] private int defaultValueWhenNotFound = 0;

    private int[,] rectangularData;

    [SerializeField] private bool convertInchesToFeet = false;

    private int debugLogCounter = 0; // Tried to use with a debug .txt file creator

    private static CSVReaderRevitDataToAStarGrid instance;
    private CSVReaderRevitDataToAStarGrid()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static CSVReaderRevitDataToAStarGrid Instance
    {
        get
        {
            if (instance == null)
            {
                new CSVReaderRevitDataToAStarGrid();
            }
            return instance;
        }
    }

    private List<DataWithPosition> fullData = new List<DataWithPosition>();

    private string[] SplitTextAssetIntoRows(TextAsset asset) => asset.text.Split(new char[] { '\n' });

    private string[] SplitRowIntoIndividualValues(string fullRow) => fullRow.Split(new char[] { ',' });
    
    private int FindDataDimension(float maxValue) => (int)(maxValue / distanceBetweenDataPoints);

    private bool isWithinDimensionBounds(int index, int dimensionBound)
    {
        return index < dimensionBound;
    }

    /*
     * Reads data from a text asset file and separates data into an organized array.
     */
    public void ReadInData()
    {
        modifyData = ModifyDataForPathingNodes.Instance;
        modifyData.DetermineCaseForCreatingDataArray(distanceBetweenDataPoints);

        TextAsset gridData = Resources.Load<TextAsset>(rhinoInputCSVName);
        int dataCounter = 1; // Starts at 1 to account for header row

        string[] data = SplitTextAssetIntoRows(gridData);
        string[] firstRow = SplitRowIntoIndividualValues(data[0]);
        int[] rowInt = new int[firstRow.Length];

        float maxX = FindMaximumValue(data, 1);
        float maxZ = FindMaximumValue(data, 2);
        Debug.Log($"File Reader found a max X of {maxX} and a max Z of {maxZ}.");

        int totalCols = FindDataDimension(maxX);
        int totalRows = FindDataDimension(maxZ);
        Debug.Log($"The file reader data dimensions are width {totalCols} by height {totalRows}.");

        rectangularData = new int[totalCols, totalRows];
        int rowIndex = 0;
        int colIndex = 0;

        while (data[dataCounter] != string.Empty && 
            isWithinDimensionBounds(rowIndex, totalRows) && 
            isWithinDimensionBounds(colIndex, totalCols))
        {
            DataWithPosition currentData;

            string[] currentRow = data[dataCounter].Split(new char[] { ',' });
            if (convertInchesToFeet)
                currentData = new DataWithPosition(currentRow[1], currentRow[2], currentRow[3], 12.0f);
            else
                currentData = new DataWithPosition(currentRow[1], currentRow[2], currentRow[3]);

            int xCoordinateNormalizedToIndex = (int)((currentData.XPosition - distanceBetweenDataPoints) / distanceBetweenDataPoints);
            int zCoordinateNormalizedToIndex = (int)((currentData.ZPosition - distanceBetweenDataPoints) / distanceBetweenDataPoints);

            bool matchingIndex = xCoordinateNormalizedToIndex == colIndex && zCoordinateNormalizedToIndex == rowIndex;
            if (matchingIndex)
            {
                rectangularData[colIndex, rowIndex] = currentData.DataValue;
                dataCounter++;
            }
            else
                rectangularData[colIndex, rowIndex] = 0;

            if (rowIndex < totalRows - 1)
                rowIndex++;
            else
            {
                rowIndex = 0;
                colIndex++;
            }
        }

        modifyData.CheckToModifyData(rectangularData);
        DebugListOut2DArray(rectangularData, "This is rect data: ");
    }


    private float FindMaximumValue(string[] dataAsRows, int columnBeingChecked)
    {
        float maximumValue = 0.0f;
        float valueToCheck = 0.0f;

        string firstRow = dataAsRows[0];
        string currentRow = firstRow;

        string[] segmentedRow;
        int rowIndex = 0;

        while (!string.IsNullOrEmpty(currentRow))
        {
            segmentedRow = SplitRowIntoIndividualValues(currentRow);
            float.TryParse(segmentedRow[columnBeingChecked], out valueToCheck);

            if(valueToCheck != 0.0f)
            {
                if (valueToCheck > maximumValue)
                    maximumValue = valueToCheck;
            }

            rowIndex++;
            currentRow = dataAsRows[rowIndex];
        }

        return maximumValue;
    }

    private void DisplayRowData(string[] row)
    {
        Debug.Log("Ref: " + row[0] + "\n" +
                        "x: " + row[1] + "\n" +
                        "y: " + row[2] + "\n" +
                        "Con: " + row[3] + "\n");
    }

    private void DisplayRowData(int[] row)
    {
        Debug.Log("Ref: " + row[0] + "\n" +
                        "x: " + row[1] + "\n" +
                        "y: " + row[2] + "\n" +
                        "Con: " + row[3] + "\n");
    }

    /*
     * Checks if passed in node indices match those of a piece of data extracted from the .csv file.
     */
    public void CheckToAssignValue(Node node)
    {
        int arrayXIndex = node.gridX;
        int arrayYIndex = node.gridY;

        if (arrayXIndex < rectangularData.GetLength(1) && arrayYIndex < rectangularData.GetLength(0))
            node.Connectivity = rectangularData[arrayXIndex, arrayYIndex];
        else
            Debug.LogWarning($"Node[{arrayXIndex}, {arrayYIndex}]: File reader attempted to assign value to a node whose index is outside of the file's data bounds.");
    }

    /*
     * Solely provides a reference on a monobehaviour class to allow a button to access the static instance of DataRecorder which 
     * is not inherited from monobehaviour.
     */
    public void GenerateOutputDataWithRecorder()
    {
        DataRecorder.Instance.OutputPathData();
    }

    #region Debugging

    private void DebugListOutStringArray(string[] array, string debugMessageStart)
    {
        string message = debugMessageStart + "\n";

        foreach (string item in array)
        {
            message += item + "\n";
        }

        //Debug.Log(message);
        ExportDataToDebugLog(message);
    }

    private void DebugListOutStringArray(float[,] array, string debugMessageStart)
    {
        string message = debugMessageStart + "\n";

        foreach (float item in array)
        {
            message += item + "\n";
        }

        Debug.Log(message);
        //ExportDataToDebugLog(message);
    }

    private void DebugListOut2DArray(float[,] array, string debugMessageStart)
    {
        string message = debugMessageStart + "\n";

        for (int i = 0; i < array.GetLength(0); i++)
        {
            message += $"Row {i}: ";

            for (int j = 0; j < array.GetLength(1); j++)
            {
                message += $" {array[j,i]}";
            }

            message += "\n";
        }

        Debug.Log(message);
        ExportDataToDebugLog(message);
    }

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

    public void ExportDataToDebugLog(string debugMessage)
    {
        string filePath = $"Assets/Resources/DebugLogDATATEST.txt";
        StreamWriter file = new StreamWriter(filePath, true);

        file.Write(debugMessage);
        file.Close();

        debugLogCounter++;
    }
    #endregion
}
