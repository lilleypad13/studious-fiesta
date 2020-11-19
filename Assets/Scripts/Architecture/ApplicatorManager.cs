using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

public class ApplicatorManager : Initializer
{
    [SerializeField] private InfluenceApplicator[] componentApplicators;
    [SerializeField] private LayerApplicator[] layerApplicators;
    
    private List<GameObject> allModelChildren = new List<GameObject>();

    public override void Initialization()
    {
        allModelChildren = GlobalModelData.Instance.ObjectsInEntireModel;
        DebugNamesOfChildren();

        UseApplicators();
    }

    /*
     * Goes through the list of Applicator objects and for each of those it searches through the entire 
     * hierarchy of children objects of the given model looking for gameobjects with a name containing the 
     * search term of that Applicator. If it finds a match, it checks if the Applicator is applying a 
     * component or a layer and adds that Applicator specified component or layer to that child gameobject.
     */
    private void UseApplicators()
    {
        ApplyComponentsToDesignatedObjects();
        ApplyLayersToDesignatedObjects();
    }

    private void ApplyLayersToDesignatedObjects()
    {
        List<GameObject> childrenWithNamesContainingSearchTerm = new List<GameObject>();

        foreach (LayerApplicator layerApplicator in layerApplicators)
        {
            string searchTerm = layerApplicator.SearchTerm;

            childrenWithNamesContainingSearchTerm = FindChildrenWithNameContain(searchTerm);

            if (childrenWithNamesContainingSearchTerm.Count != 0)
            {
                foreach (GameObject child in childrenWithNamesContainingSearchTerm)
                {
                    child.layer = Mathf.RoundToInt(Mathf.Log(layerApplicator.LayerToApply.value, 2));
                }
            }
            else
                Debug.Log($"{this.name} did not find any children objects containing the term {searchTerm}.");
        }
    }

    private void ApplyComponentsToDesignatedObjects()
    {
        List<GameObject> childrenWithNamesContainingSearchTerm = new List<GameObject>();

        foreach (InfluenceApplicator applicator in componentApplicators)
        {
            string searchTerm = applicator.SearchTerm;
            Type componentToAdd = applicator.InfluenceComponentToApply.Type;

            childrenWithNamesContainingSearchTerm = FindChildrenWithNameContain(searchTerm);

            if (childrenWithNamesContainingSearchTerm.Count != 0)
            {
                foreach (GameObject child in childrenWithNamesContainingSearchTerm)
                {
                    TypeReference componentAdding = applicator.InfluenceComponentToApply;
                    if (child.GetComponent<Influence>() == null)
                    {
                        child.AddComponent(componentAdding);
                        child.GetComponent<Influence>().InfluenceAmount = applicator.InfluenceAmount;
                    }
                    else
                        Debug.LogWarning($"{this.name} tried to add component {componentAdding.Type} to {child.name}, but it already has an Influence component.");
                }
            }
            else
                Debug.Log($"{this.name} did not find any children objects containing the term {searchTerm}.");
        }
    }

    private List<GameObject> FindChildrenWithNameContain(string searchTerm)
    {
        List<GameObject> childList = new List<GameObject>();
        string capitalizedSearchTerm = searchTerm.ToUpper();

        foreach (GameObject child in allModelChildren)
        {
            if (child.name.ToUpper().Contains(capitalizedSearchTerm))
                childList.Add(child);
        }

        return childList;
    } 

    

    #region DebugMethods

    /*
     * Debug method to get the names of all the children objects of the model gameobject
     */
    public void DebugNamesOfChildren()
    {
        string listOfChildrenNames = "List of Children: \n";

        foreach (GameObject child in allModelChildren)
        {
            listOfChildrenNames += child.name + "\n";
        }

        Debug.Log(listOfChildrenNames);
    }

    private void DebugListOfObjects(List<GameObject> list)
    {
        string debugMessage = "This list contains: \n";

        foreach (GameObject item in list)
        {
            debugMessage += item.name + "\n";
        }

        Debug.Log(debugMessage);
    }

    #endregion
}
