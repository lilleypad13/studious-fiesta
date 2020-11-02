using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicatorManager : Initializer
{
    [SerializeField] private GameObject model;
    [SerializeField] private ComponentApplicator[] componentApplicators;
    [SerializeField] private LayerApplicator[] layerApplicators;
    

    private List<GameObject> allModelChildren = new List<GameObject>();

    // Need a new object type that will hold information on applicators
    // private Applicator[] appList;

    //private void Awake()
    //{
    //    GetChildrenGameObjects();
    //    DebugNamesOfChildren();

    //    UseApplicators();
    //}

    /*
     * Sets allModelChildren list to all of the children gameobjects in the main model gameobject
     */
    private void GetChildrenGameObjects()
    {
        foreach (Transform child in model.transform)
        {
            allModelChildren.Add(child.gameObject);
        }
    }

    /*
     * Goes through the list of Applicator objects and for each of those it searches through the entire 
     * hierarchy of children objects of the given model looking for gameobjects with a name containing the 
     * search term of that Applicator. If it finds a match, it checks if the Applicator is applying a 
     * component or a layer and adds that Applicator specified component or layer to that child gameobject.
     */
    private void UseApplicators()
    {
        foreach (ComponentApplicator componentApplicator in componentApplicators)
        {
            foreach (GameObject child in allModelChildren)
            {
                if (child.name == componentApplicator.SearchTerm)
                    child.AddComponent(componentApplicator.InfluenceComponentToApply);
            }
        }

        foreach (LayerApplicator layerApplicator in layerApplicators)
        {
            foreach (GameObject child in allModelChildren)
            {
                if (child.name == layerApplicator.SearchTerm)
                {
                    Debug.Log("The layer value is: " + layerApplicator.LayerToApply.value);
                    child.layer = Mathf.RoundToInt(Mathf.Log(layerApplicator.LayerToApply.value, 2));
                }
            }
        }

        // Psuedocode for how this method will be used
        /*
        foreach (Applicator app in appList)
        {
            foreach (Gameobject child in allModelChildren)
            {
                if(child.name.Contains(app.searchTerm)
                {
                    if(app.compOrLayer == component)
                        child.AddComponent(app.componentType)
                    else
                        child.layer = app.layerInt;
                }
            }
        }
        */
    }

    public override void Initialization()
    {
        GetChildrenGameObjects();
        DebugNamesOfChildren();

        UseApplicators();
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

    #endregion
}
