using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlobalModelData
{
    private static GlobalModelData instance = null;
    private static readonly object padlock = new object();

    public static GlobalModelData Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                    instance = new GlobalModelData();
            }
            return instance;
        }
    }

    private Bounds modelBounds;
    public Bounds ModelBounds
    {
        get => modelBounds;
        set => modelBounds = value;
    }

    private List<GameObject> objectsInEntireModel;
    public List<GameObject> ObjectsInEntireModel
    {
        get => objectsInEntireModel;
        set
        {
            if (objectsInEntireModel == null)
                objectsInEntireModel = value;
            else
                Debug.LogWarning("An attempt at changing the list of objects in the entire model was made.");
        }
    }

    public GameObject SearchEntireModelForObjectWithNameContaining(string searchID)
    {
        Debug.Log($"Searching entire model for object containing ID: {searchID}");
        foreach (GameObject item in GlobalModelData.Instance.ObjectsInEntireModel)
        {
            if (item.name.Contains(searchID))
                return item;
        }

        Debug.LogWarning("Revit model data contained an ID number which was not found in the current model.");
        return null;
    }
}
