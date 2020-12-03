using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SheetWithColumnNames
{
    [SerializeField]private string sheetName;
    public string SheetName { get => sheetName; }
    [SerializeField]private string[] columnNames;
    public string[] ColumnNames { get => columnNames; }

    public SheetWithColumnNames(string _sheetName, string[] _columnNames)
    {
        sheetName = _sheetName;
        columnNames = _columnNames;
    }
}

public class CSVReaderRevitModel : MonoBehaviour
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
    [SerializeField] private SheetWithColumnNames[] sheetsToRead;

    private void TestCreateDataArray()
    {
        List<string[,]> allDataArrays = new List<string[,]>();

        foreach (SheetWithColumnNames item in sheetsToRead)
        {
            string filePath = nameFolderRevitModelCSVs + "/" + item.SheetName;

            allDataArrays.Add(CSVReader.Instance.ReadCSVFileTo2DStringArray(filePath));
        }

        int sheetCounter = 0;
        foreach (string[,] array in allDataArrays)
        {
            CSVReader.Instance.DebugListOut2DArray(array, sheetsToRead[sheetCounter].SheetName);
            sheetCounter++;
        }
    }

    private void Start()
    {
        TestCreateDataArray();
    }
}
