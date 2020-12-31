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

    public Bounds ModelBounds
    {
        get => modelBounds;
        set => modelBounds = value;
    }
    private Bounds modelBounds;

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
    private List<GameObject> objectsInEntireModel;

    public static Dictionary<string, ArchitecturalElementContainer> architecturalElementContainers = new Dictionary<string, ArchitecturalElementContainer>();

    public ArchitecturalElementContainer GetFromContainerDictionary(string key)
    {
        if (architecturalElementContainers.ContainsKey(key))
            return architecturalElementContainers[key];
        else
            return null;
    }

    public void AddIfNotInDictionary(string key)
    {
        if (!architecturalElementContainers.ContainsKey(key))
        {
            architecturalElementContainers.Add(key, new ArchitecturalElementContainer(key));
            Debug.Log($"{key} architectural type added to global architectural element dictionary.");
        }
    }

    public void CheckValueAgainstArchitecturalElementContainer(ArchitecturalElement element)
    {
        if (architecturalElementContainers.ContainsKey(element.Name))
        {
            architecturalElementContainers[element.Name].CheckValueAgainstMinAndMax(element.ArchitecturalValue);
        }
    }

    public GameObject SearchEntireModelForObjectWithNameContaining(string searchID)
    {
        //Debug.Log($"Searching entire model for object containing ID: {searchID}");
        foreach (GameObject item in GlobalModelData.Instance.ObjectsInEntireModel)
        {
            if (item.name.Contains(searchID))
                return item;
        }

        Debug.LogWarning("Revit model data contained an ID number which was not found in the current model.");
        return null;
    }

    public Vector3 GetPositionByBounds(GameObject objectWithPosition)
    {
        Renderer objectsRenderer = objectWithPosition.GetComponent<Renderer>();

        if(objectsRenderer != null)
            return objectsRenderer.bounds.center;
        else
        {
            Debug.LogWarning("Tried to find the position of an object based on bounds that has no renderer component.");
            return Vector3.zero;
        }
    }

    #region Debugging

    public void ReportArchitecturalDictionary()
    {
        string debugMessage = "Global Architectural Element Dictionary: \n";

        foreach (KeyValuePair <string, ArchitecturalElementContainer> container in architecturalElementContainers)
        {
            debugMessage += $"Architecture Type: {container.Key}; Min: {container.Value.MinimumValue}; Max: {container.Value.MaximumValue}\n";
        }

        Debug.Log(debugMessage);
    }

    #endregion
}
