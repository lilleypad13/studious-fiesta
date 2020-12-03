using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader
{
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
    private string[] SplitTextAssetIntoRows(TextAsset asset) => asset.text.Split(new char[] { '\n' });

    private string[] SplitRowIntoIndividualValues(string fullRow) => fullRow.Split(new char[] { ',' });
    
    /*
     * Reads through an entire CSV file and converts it into a 2D array of string data
     */
    public string[,] ReadCSVFileTo2DStringArray(string inputCSVFileName)
    {
        TextAsset csvAsset = Resources.Load<TextAsset>(inputCSVFileName);
        int dataCounter = 0;

        string[] data = SplitTextAssetIntoRows(csvAsset); // Splits entirety of .csv file into rows, each encapsulated by a single string
        string headerRow = data[0];

        // Finds dimensions for output 2D array
        // Width determined by the first row (generally the header row)
        // Height determined by how many elements there are after the first split of data (based on new line character)
        int dataWidth = FindWidthOfData(headerRow);
        int dataHeight = data.Length - 1; // Length normally provides an extra row because the final line also has a new line character

        Debug.Log($"Data Width is: {dataWidth} and Data Height is: {dataHeight}.");

        // Initializes array to be filled with data that will be the return of the method
        string[,] fullySeparated2DDataArray = new string[dataWidth, dataHeight];

        // Goes through entirety of csv file (until it reaches an empty row)
        // Breaks the rows into individual string elements for each cell/separated value
        // Places that broken up data into the output 2D array an entire row at a time
        while (data[dataCounter] != string.Empty)
        {
            // Breaks the current single string row into its individual string values for each cell/separated value
            string[] currentRow = SplitRowIntoIndividualValues(data[dataCounter]);

            // Populates entire row of fullySeparated2DDataArray using all the values 
            // in the newly separated currentRow
            for (int i = 0; i < dataWidth; i++)
            {
                fullySeparated2DDataArray[i, dataCounter] = currentRow[i];
            }

            dataCounter++;
        }

        return fullySeparated2DDataArray;
    }

    /*
     * Takes any row of data from the CSV file to return the width (how many columns) of the data
     */
    private int FindWidthOfData(string rowOfData)
    {
        string[] row = SplitRowIntoIndividualValues(rowOfData);
        int width = row.Length;

        return width;
    }

    #region Debugging

    /*
     * Outputs debug.log message of the entirety of a 2D string array
     * Output is a single message where each row is separated by a new line and individual elements
     * are separated by a single space.
     */
    public void DebugListOut2DArray(string[,] array, string debugMessageFirstLine)
    {
        string message = debugMessageFirstLine + "\n";

        for (int i = 0; i < array.GetLength(1); i++)
        {
            message += $"Row {i}: ";

            for (int j = 0; j < array.GetLength(0); j++)
            {
                message += $" {array[j, i]}";
            }

            message += "\n";
        }

        Debug.Log(message);
    }

    #endregion
}
