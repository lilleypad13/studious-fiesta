using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjectGatherer
{
    private List<GameObject> allObjectChildren = new List<GameObject>();

    public List<GameObject> CreateListOfChildrenObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform.GetComponentsInChildren<Transform>())
        {
            allObjectChildren.Add(child.gameObject);
        }

        return allObjectChildren;
    }

    private void AddChildrenToList(GameObject parent, List<GameObject> listToAddChildrenTo)
    {
        if(parent.transform.childCount > 0)
        {
            foreach (Transform child in parent.transform)
            {
                listToAddChildrenTo.Add(child.gameObject);
            }
        }
    }
    
}
