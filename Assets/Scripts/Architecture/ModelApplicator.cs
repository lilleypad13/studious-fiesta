using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelApplicator : MonoBehaviour
{
    [SerializeField] private GameObject model;
    private List<GameObject> allModelChildren = new List<GameObject>();

    // Need a new object type that will hold information on applicators
    // private Applicator[] appList;

    private void Awake()
    {
        GetChildrenGameObjects();
    }

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
}
