using System;
using System.IO;
using UnityEngine;

[Serializable]
public class CSVFile
{
    public string DataName { get => dataName; set => dataName = value; }
    [SerializeField]private string dataName = "";

    public string FilePath { get => filePath; }
    [SerializeField]private string filePath = "";

    public CSVFile(string _dataName, string _filePath)
    {
        dataName = _dataName;
        filePath = _filePath;
    }

}

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
    [SerializeField] private CSVFile csv;
    public CSVFile CSV { get => csv; }
    public string DataType { get => csv.DataName; }


    [Header("File Parameters")]
    [Tooltip("General distance between each data point in the file to be read.")]
    [SerializeField] private float distanceBetweenDataPoints = 1.0f; // Increment value between coordinates of each data point
    [Tooltip("Value to assign empty coordinate locations not found in the file.")]
    [SerializeField] private int defaultValueWhenNotFound = 0;
    [SerializeField] private bool convertInchesToFeet = false;

    [Header("Coordinate Columns")]
    [Tooltip("Column x-coordinate is found in the file to be read.")]
    [SerializeField] private int xCoordinateColumnIndex = 1;
    [Tooltip("Column y-coordinate (or z-coordinate) is found in the file to be read.")]
    [SerializeField] private int yCoordinateColumnIndex = 2;

    [Header("Data Alignment")]
    [SerializeField] private bool flipDataReadingHorizontally = false;
    [SerializeField] private bool flipDataReadingVertically = false;

    private float minX = 0.0f;
    private float maxX = 0.0f;
    private float minZ = 0.0f;
    private float maxZ = 0.0f;

    private int[,] rectangularData;
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

    // Expression-bodied Members
    private string[] SplitTextAssetIntoRows(TextAsset asset) => asset.text.Split(new char[] { '\n' });

    private string[] SplitRowIntoIndividualValues(string fullRow) => fullRow.Split(new char[] { ',' });

    // Data creation methods
    private int FindDataDimension(float minValue, float maxValue)
    {
        int dataDimension = 0;

        dataDimension = (int)(((maxValue - minValue) / distanceBetweenDataPoints) + 1);

        return dataDimension;
    }

    private int NormalizeCoordinateToIndex(float coordinate, float minValue)
    {
        int normalizedIndex = 0;

        normalizedIndex = (int)((Mathf.Abs(coordinate) - minValue) / distanceBetweenDataPoints);

        return normalizedIndex;
    }

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

        TextAsset gridData = Resources.Load<TextAsset>(csv.FilePath);
        int dataIndex = 1; // Starts at 1 to account for header row

        string[] data = SplitTextAssetIntoRows(gridData);
        string[] firstRow = SplitRowIntoIndividualValues(data[0]);
        int[] rowInt = new int[firstRow.Length];

        if (csv.DataName == "")
            csv.DataName = firstRow[firstRow.Length - 1];
        GlobalModelData.Instance.CheckIfAlreadyInDictionary(csv.DataName);

        FindExtremeValues(data);
        Debug.Log($"File Reader found a max X of {maxX} and a max Z of {maxZ}.");

        int totalCols = FindDataDimension(minX, maxX);
        int totalRows = FindDataDimension(minZ, maxZ);
        Debug.Log($"The file reader data dimensions are width {totalCols} by height {totalRows}.");

        rectangularData = new int[totalCols, totalRows];
        int colIndex = 0;
        int rowIndex = 0;
        int assignedColIndex = 0;
        int assignedRowIndex = 0;
        DataWithPosition currentData = null;
        int previousDataIndex = 0;
        int valueToAssignToData = defaultValueWhenNotFound;

        string newDebugMessage = "";

        while (data[dataIndex] != string.Empty && 
            isWithinDimensionBounds(rowIndex, totalRows) && 
            isWithinDimensionBounds(colIndex, totalCols))
        {
            string[] currentRow = data[dataIndex].Split(new char[] { ',' });
            int endOfRowIndex = currentRow.Length - 1;

            DataWithPosition previousData = null;
            if (currentData != null)
                previousData = currentData;

            if (convertInchesToFeet)
                currentData = new DataWithPosition(currentRow[xCoordinateColumnIndex], currentRow[yCoordinateColumnIndex], currentRow[endOfRowIndex], 12.0f);
            else
                currentData = new DataWithPosition(currentRow[xCoordinateColumnIndex], currentRow[yCoordinateColumnIndex], currentRow[endOfRowIndex]);

            bool isRepeatingCoordinates = false;
            bool isRepeatingDataIndex = dataIndex == previousDataIndex;
            if (previousData != null && !isRepeatingDataIndex)
                isRepeatingCoordinates = currentData.XPosition == previousData.XPosition && currentData.ZPosition == previousData.ZPosition;

            if (!isRepeatingCoordinates)
            {
                int xCoordinateNormalizedToIndex = NormalizeCoordinateToIndex(currentData.XPosition, minX);
                int zCoordinateNormalizedToIndex = NormalizeCoordinateToIndex(currentData.ZPosition, minZ);

                assignedColIndex = colIndex;
                assignedRowIndex = rowIndex;

                bool matchingIndex = xCoordinateNormalizedToIndex == colIndex && zCoordinateNormalizedToIndex == rowIndex;
                if (matchingIndex)
                {
                    valueToAssignToData = currentData.DataValue;
                    previousDataIndex = dataIndex;
                    dataIndex++;
                }
                else
                {
                    previousDataIndex = dataIndex;
                    valueToAssignToData = defaultValueWhenNotFound;
                }

                if(flipDataReadingHorizontally)
                    assignedColIndex = (totalCols - 1) - assignedColIndex;
                if(flipDataReadingVertically)
                    assignedRowIndex = (totalRows - 1) - assignedRowIndex;
                rectangularData[assignedColIndex, assignedRowIndex] = valueToAssignToData;

                if (rowIndex < totalRows - 1)
                    rowIndex++;
                else
                {
                    rowIndex = 0;
                    colIndex++;
                }
            }
            else
            {
                previousDataIndex = dataIndex;
                dataIndex++;
            }

            //if(previousData != null)
            //{
            //    newDebugMessage += $"DataX: {currentData.XPosition}, " +
            //    $"PreviousDataX: {previousData.XPosition}, " +
            //    $"DataY: {currentData.ZPosition}, " +
            //    $"PreviousDataY: {previousData.ZPosition}, " +
            //    $"Value: {currentData.DataValue}, " +
            //    $"colIndex: {colIndex}, " +
            //    $"rowIndex: {rowIndex}, " +
            //    $"dataIndex: {dataIndex}, " +
            //    $"prevDataIndex: {previousDataIndex}" + "\n";
            //}
            
        }

        modifyData.CheckToModifyData(rectangularData);
        DebugListOut2DArray(rectangularData, "This is rect data: ", "RevitPathingCSV_02");
    }


    private void FindExtremeValues(string[] dataAsRows)
    {
        float xValueToCheck = 0.0f;
        float zValueToCheck = 0.0f;

        string firstRow = dataAsRows[0];
        string currentRow = firstRow;

        string[] segmentedRow;
        int rowIndex = 0;

        while (!string.IsNullOrEmpty(currentRow))
        {
            segmentedRow = SplitRowIntoIndividualValues(currentRow);

            float.TryParse(segmentedRow[xCoordinateColumnIndex], out xValueToCheck);
            if (xValueToCheck != 0.0f)
            {
                if (Mathf.Abs(xValueToCheck) > maxX)
                    maxX = Mathf.Abs(xValueToCheck);
                else if (Mathf.Abs(xValueToCheck) < minX)
                    minX = Mathf.Abs(xValueToCheck);
            }

            float.TryParse(segmentedRow[yCoordinateColumnIndex], out zValueToCheck);
            if (zValueToCheck != 0.0f)
            {
                if (Mathf.Abs(zValueToCheck) > maxZ)
                    maxZ = Mathf.Abs(zValueToCheck);
                else if (Mathf.Abs(zValueToCheck) < minZ)
                    minZ = Mathf.Abs(zValueToCheck);
            }

            rowIndex++;
            currentRow = dataAsRows[rowIndex];
        }
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
     * Solely provides a reference on a monobehaviour class to allow a button to access the static instance of DataRecorder which 
     * is not inherited from monobehaviour.
     */
    public void GenerateOutputDataWithRecorder()
    {
        DataRecorder.Instance.OutputPathData();
    }

    #region Debugging

    private void DebugListOutStringArray(string[] array, string debugMessageStart, string debugLogFileName)
    {
        string message = debugMessageStart + "\n";

        foreach (string item in array)
        {
            message += item + "\n";
        }

        //Debug.Log(message);
        ExportDataToDebugLog(message, debugLogFileName);
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

    private void DebugListOut2DArray(float[,] array, string debugMessageStart, string debugLogFileName)
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
        ExportDataToDebugLog(message, debugLogFileName);
    }

    private void DebugListOut2DArray(int[,] array, string debugMessageStart, string debugLogFileName)
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
        ExportDataToDebugLog(message, debugLogFileName);
    }

    public void ExportDataToDebugLog(string debugMessage, string fileName)
    {
        string filePath = $"Assets/Resources/DebugLogs/{fileName}.txt";
        StreamWriter file = new StreamWriter(filePath, true);

        file.Write(debugMessage);
        file.Close();

        debugLogCounter++;
    }
    #endregion
}
