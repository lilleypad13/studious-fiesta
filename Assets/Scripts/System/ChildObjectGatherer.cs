using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjectGatherer
{
    private List<GameObject> allObjectChildren = new List<GameObject>();

    public List<GameObject> CreateListOfChildrenObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            allObjectChildren.Add(child.gameObject);
        }

        return allObjectChildren;
    }
}
