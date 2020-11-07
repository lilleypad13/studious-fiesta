using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjectGatherer
{
    [SerializeField] private GameObject parentToGatherChildObjects;
    private List<GameObject> allObjectChildren = new List<GameObject>();
    public List<GameObject> AllObjectChildren
    {
        get => allObjectChildren;
        set => allObjectChildren = value;
    }

    public void CreateListOfChildrenObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            allObjectChildren.Add(child.gameObject);
        }
    }
}
