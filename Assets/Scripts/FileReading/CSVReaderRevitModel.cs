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
    #region Singleton Pattern
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
    #endregion

    [Header("Files to Read")]
    [Tooltip("Name of folder in Unity's Resources folder which holds the .csv files corresponding to Revit models.")]
    [SerializeField] private string nameFolderRevitModelCSVs = "RevitModelCSVs";

    [SerializeField] private string columnHeaderInfluenceId = "ValueGroup";
    private string influencerHandlerId = "Influence";
    [SerializeField] private string columnHeaderWalkableId = "Unwalkable";
    private string walkabilityHandlerId = "Walkable";
    [SerializeField] private string columnHeaderSpawnId = "SpawnTarget";
    private string spawnerHandlerId = "Spawn";
    [SerializeField] private string columnHeaderSelectableId = "Selectable";
    private string selectableHandlerId = "Select";

    private static Dictionary<string, string[,]> sheetDataDictionary = new Dictionary<string, string[,]>();

    // Debugging Fields
    private string debugInfo = "";


    public override void Initialization()
    {
        sheetDataDictionary = CreateDataArraysFromCSVs();

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
        for (int i = 0; i < rowOfStrings.Length; i++)
        {
            if (rowOfStrings[i].Contains(searchTerm))
            {
                return i;
            }
        }

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
            FindColumnAndApplyDataHandler(dataArray.Key, columnHeaderSelectableId, selectableHandlerId);
        }

        Debug.Log("CSVReaderRevitModel Data: \n" + debugInfo);
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
            }
            debugInfo += $"{nameDataArray} data: {columnHeaderId} column found. \n";
        }
        else
            debugInfo += $"{nameDataArray} data: {columnHeaderId} column NOT found. \n";
    }

    #region Debugging
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
