using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class SheetAndColumn
{
    [Tooltip("Exact sheet name.")]
    [SerializeField] private string sheetName;
    public string SheetName { get => sheetName; }
    [Tooltip("Exact column header where values of interest are located.")]
    [SerializeField] private string columnName;
    public string ColumnName { get => columnName; }

    public SheetAndColumn(string _sheetName, string _columnName)
    {
        sheetName = _sheetName;
        columnName = _columnName;
    }
}

public class CSVReaderRevitModel : Initializer
{
    private static CSVReaderRevitModel instance;
    private CSVReaderRevitModel()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static CSVReaderRevitModel Instance
    {
        get
        {
            if (instance == null)
            {
                new CSVReaderRevitModel();
            }
            return instance;
        }
    }

    [Header("Files to Read")]
    [Tooltip("Name of folder in Unity's Resources folder which holds the .csv files corresponding to Revit models.")]
    [SerializeField] private string nameFolderRevitModelCSVs = "RevitModelCSVs";
    [Tooltip("List of exact sheet names to read model data from")]
    [SerializeField] private SheetAndColumn[] sheetsOfInterest;

    [SerializeField] private string columnHeaderInfluenceId = "ValueGroup";
    private string influencerHandlerId = "Influence";
    [SerializeField] private string columnHeaderWalkableId = "Unwalkable";
    private string walkabilityHandlerId = "Walkable";
    [SerializeField] private string columnHeaderSpawnId = "SpawnTarget";
    private string spawnerHandlerId = "Spawn";

    private static Dictionary<string, string[,]> sheetDataDictionary = new Dictionary<string, string[,]>();


    public override void Initialization()
    {
        sheetDataDictionary = CreateDataArraysFromCSVs();

        if (sheetsOfInterest == null)
            Debug.LogWarning("No Data was read in from Revit Data to apply to models.");
        else
            ApplyDataToModel();

        DebugDisplay2DArrayStringDictionary(sheetDataDictionary);
    }


    private Dictionary<string, string[,]> CreateDataArraysFromCSVs()
    {
        Dictionary<string, string[,]> allDataDictionary = new Dictionary<string, string[,]>();

        UnityEngine.Object[] csvFiles = Resources.LoadAll(nameFolderRevitModelCSVs, typeof(TextAsset));

        foreach (UnityEngine.Object file in csvFiles)
        {
            string filePath = nameFolderRevitModelCSVs + "/" + file.name;
            allDataDictionary.Add(file.name, CSVReader.Instance.ReadCSVFileTo2DStringArray(filePath));
            Debug.Log($"Added csv file to dictionary: {file.name}");
        }

        return allDataDictionary;
    }


    private int FindColumnIndexWithString(string[] rowOfStrings, string searchTerm)
    {
        Debug.Log($"Find column index searched: {searchTerm} within the elements {ListOfArrayStrings(rowOfStrings)}.");

        for (int i = 0; i < rowOfStrings.Length; i++)
        {
            if (rowOfStrings[i].Contains(searchTerm))
            {
                Debug.Log("FOUND RESULT.");
                return i;
            }
        }

        Debug.Log($"Could not find {searchTerm} in headers.");
        return 0;
    }


    private int SearchHeadersForColumnIndex(string[,] data, string[] headingRow, string headerSearchingFor)
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            headingRow[i] = data[i, 0];
        }

        int columnIndex = FindColumnIndexWithString(headingRow, headerSearchingFor);
        return columnIndex;
    }


    public static string[,] FindWithinDictionary(string searchTerm)
    {
        string[,] array;
        sheetDataDictionary.TryGetValue(searchTerm, out array);

        return array;
    }
    

    private void ApplyDataToModel()
    {
        // Checks to do for ALL Csv files
        foreach (KeyValuePair<string, string[,]> dataArray in sheetDataDictionary)
        {
            FindColumnAndApplyDataHandler(dataArray.Key, columnHeaderWalkableId, walkabilityHandlerId);
            FindColumnAndApplyDataHandler(dataArray.Key, columnHeaderInfluenceId, influencerHandlerId);
            FindColumnAndApplyDataHandler(dataArray.Key, columnHeaderSpawnId, spawnerHandlerId);
        }

        //foreach(SheetAndColumn item in sheetsOfInterest)
        //{
        //    Debug.Log($"CSVReaderRevitModel trying to apply data based on sheet: {item.SheetName}.");
        //    string[,] data = FindWithinDictionary(item.SheetName);
        //    string[] headingRow = new string[data.GetLength(0)];
        //    int columnIndex = SearchHeadersForColumnIndex(data, headingRow, item.ColumnName);

        //    for (int i = 1; i < data.GetLength(1); i++)
        //    {
        //        string idNumber = data[0, i];
        //        GameObject modelToModify = GlobalModelData.Instance.SearchEntireModelForObjectWithNameContaining(idNumber);
        //        if (modelToModify != null)
        //            RevitModelDataHandlerManager.Instance.ApplyHandlerMethodBasedOnString(item.SheetName, modelToModify, data[columnIndex, i]);
        //        else
        //            Debug.Log($"Did not find an object within the model associated with ID: {idNumber}.");
        //    }
        //}
    }


    private void FindColumnAndApplyDataHandler(string nameDataArray, string columnHeaderId, string handlerId)
    {
        string[,] data = FindWithinDictionary(nameDataArray);
        string[] headingRow = new string[data.GetLength(0)];
        int columnIndex = SearchHeadersForColumnIndex(data, headingRow, columnHeaderId);

        if (columnIndex != 0)
        {
            for (int i = 1; i < data.GetLength(1); i++)
            {
                string idNumber = data[0, i];
                GameObject modelToModify = GlobalModelData.Instance.SearchEntireModelForObjectWithNameContaining(idNumber);
                if (modelToModify != null)
                    RevitModelDataHandlerManager.Instance.ApplyHandlerMethodBasedOnString(handlerId, modelToModify, data[columnIndex, i]);
                //else
                //    Debug.Log($"Did not find an object within the model associated with ID: {idNumber}.");
            }
        }
        else
            Debug.Log($"Column {columnHeaderId} was not found in data of: {nameDataArray}");
    }

    #region Debugging

    private void DebugDisplaySheetsData(List<string[,]> allDataArrays)
    {
        int sheetCounter = 0;
        foreach (string[,] array in allDataArrays)
        {
            CSVReader.Instance.DebugListOut2DArray(array, sheetsOfInterest[sheetCounter].SheetName);
            sheetCounter++;
        }
    }

    private void DebugDisplay2DArrayStringDictionary(Dictionary<string, string[,]> dictionary)
    {
        foreach (KeyValuePair<string, string[,]> kvp in dictionary)
        {
            CSVReader.Instance.DebugListOut2DArray(kvp.Value, $"Key: {kvp.Key}");
        }
    }

    private string ListOfArrayStrings(string[] array)
    {
        string listAsSingleString = "";

        foreach (string item in array)
        {
            listAsSingleString += item + " ";
        }

        return listAsSingleString;
    }
    
    #endregion
}
