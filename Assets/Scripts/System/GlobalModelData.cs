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

    public List<GameObject> spawnObjects = new List<GameObject>();
    public List<GameObject> targetObjects = new List<GameObject>();

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
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
        {
            if (!architecturalElementContainers.ContainsKey(key))
            {
                architecturalElementContainers.Add(key, new ArchitecturalElementContainer(key));
            }
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
        foreach (GameObject item in GlobalModelData.Instance.ObjectsInEntireModel)
        {
            if (item.name.Contains(searchID))
                return item;
        }

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

    public void AddToSpawnObjects(GameObject spawn)
    {
        spawnObjects.Add(spawn);
    }

    public void AddToTargetObjects(GameObject target)
    {
        targetObjects.Add(target);
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
