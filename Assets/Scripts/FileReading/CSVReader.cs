using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;

public class DataWithPosition
{
    int xPosition;
    int zPosition;
    int dataValue;

    public DataWithPosition(int x, int z, int data)
    {
        xPosition = x;
        zPosition = z;
        dataValue = data;
    }

}

public class CSVReader : MonoBehaviour
{
    // Data about the file being read
    [Header("File to Read")]
    [Tooltip("File name that can be copied from any file in the Resources folder.")]
    [SerializeField] private string rhinoInputCSVName = "DemoModel_VGACoordinates";

    [Header("File Parameters")]
    [Tooltip("EXPERIMENTAL: Generally keep at 1 (unless weird data set).")]
    [SerializeField] private int dataIncrement = 1; // Increment value between coordinates of each data point
    // dataWidth could be automated by finding the largest x-value
    [Tooltip("Value to assign empty coordinate locations not found in the file.")]
    [SerializeField] private int defaultValueWhenNotFound = 0;

    [SerializeField] private bool convertInchesToFeet = false;

    // Data to hold for use within Unity
    private static CSVReader instance;
    private CSVReader()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static CSVReader Instance
    {
        get
        {
            if (instance == null)
            {
                new CSVReader();
            }
            return instance;
        }
    }

    //public int[,,] FullData
    //{
    //    get { return fullData; }
    //}
    //private int[,,] fullData;
    private List<DataWithPosition> fullData = new List<DataWithPosition>();

    /*
     * Reads data from a text asset file and separates data into an organized array.
     */
    public void ReadInData()
    {
        TextAsset gridData = Resources.Load<TextAsset>(rhinoInputCSVName);
        int dataCounter = 1; // Starts at 1 to account for header row

        string[] data = gridData.text.Split(new char[] { '\n' }); // TODO: Check if this is giving +/- 1 extra piece of data
        string[] row = data[0].Split(new char[] { ',' }); // Counts number of elements in first row to determine how many columns data has
        int[] rowInt = new int[row.Length];
        Debug.Log($"The number of rows in the data is: {data.Length} and the number of columns in the data is: {row.Length}.");
        DebugListOutStringArray(data, "This is the data string array:");

        while (data[dataCounter] != string.Empty)
        {
            DataWithPosition currentData;

            string[] currentRow = data[dataCounter].Split(new char[] { ',' });
            if(convertInchesToFeet)
                currentData = new DataWithPosition(int.Parse(currentRow[1]) / 12, int.Parse(currentRow[2]) / 12, int.Parse(currentRow[3]) / 12);
            else
                currentData = new DataWithPosition(int.Parse(currentRow[1]), int.Parse(currentRow[2]), int.Parse(currentRow[3]));

            fullData.Add(currentData);

            dataCounter++;
        }

        Debug.Log($"The number of elements in fullData list is: {fullData.Count}.");

        //for(int i = 0; i < dataWidth; i++)
        //{
        //    for (int j = 0; j < dataHeight; j++)
        //    {
        //        row = data[dataCounter].Split(new char[] { ',' });

        //        // Converts row of string data from file into an int array
        //        for (int k = 0; k < row.Length; k++)
        //        {
        //            int.TryParse(row[k], out rowInt[k]);
        //        }

        //        // Checks if current data row in file has coordinates which matchup with an array element 
        //        // in the generated data array.
        //        // If it matches, it assigns that entire row's data to that row in the data array and moves the process 
        //        // forward to assign the next row of data from the file.
        //        // If they do not match, it creates a new row of data with the coordinates not found in the 
        //        // read file and assigns it a designated default "not found" value, and retains the same row 
        //        // of data until it finds where to place it.
        //        if(rowInt[1] == (i + 1) * dataIncrement && rowInt[2] == (j + 1) * dataIncrement)
        //        {
        //            for (int k = 0; k < row.Length; k++)
        //            {
        //                fullData[i, j, k] = rowInt[k];
        //            }
        //            dataCounter++;
        //        }
        //        else
        //        {
        //            rowInt[1] = i + 1;
        //            rowInt[2] = j + 1;
        //            rowInt[3] = defaultValueWhenNotFound;

        //            for (int k = 0; k < row.Length; k++)
        //            {
        //                fullData[i, j, k] = rowInt[k];
        //            }
        //        }
        //    }
        //}
    }

    /*
     * Displays organized data from a specific row within the file data
     */
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
     * Checks if passed in node coordinates match those of the current piece of data from that extracted from the .csv file.
     * If they match, it properly assigns its data to the data of the node and advances the .csv file data to the next line.
     * If they do not match, no data is assigned from the .csv file and it remains on the same line (waiting to find the matching 
     * node to assign its data to).
     */
    //public void CheckToAssignValue(Node node)
    //{
    //    int arrayXIndex = node.gridX;
    //    int arrayYIndex = node.gridY;

    //    node.Connectivity = fullData[arrayXIndex, arrayYIndex, 3];
    //}

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

    public void ExportDataToDebugLog(string debugMessage)
    {
        string filePath = "Assets/Resources/DebugLog.txt";
        StreamWriter file = new StreamWriter(filePath, true);

        file.Write(debugMessage);
        file.Close();
    }
    #endregion
}
