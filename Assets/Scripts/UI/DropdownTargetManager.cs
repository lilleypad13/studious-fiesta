using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownTargetManager : DropdownManager
{
    [SerializeField] private AgentSpawnManager spawnManager;
    [SerializeField] private Material targetHighlightMaterial;
    [SerializeField] GameObject targetMarker;
    [SerializeField] float markerHeight = 10.0f;

    List<GameObject> targetPoints = new List<GameObject>();
    private GameObject targetObject;

    private Material originalMaterial;
    private Selectable selection;

    protected override void Start()
    {
        base.Start();

        if (spawnManager.AllObjectsAsSpawnTargets)
            targetPoints = GlobalModelData.Instance.ObjectsInEntireModel;
        else
            targetPoints = GlobalModelData.Instance.targetObjects;

        foreach (GameObject item in targetPoints)
        {
            AddToDropdownList(item.name);
        }
    }


    protected override void DropdownItemSelected(Dropdown dropdown)
    {
        base.DropdownItemSelected(dropdown);
    }


    protected override void MethodToPerformOnSelection(int index)
    {
        spawnManager.Target = GlobalModelData.Instance.GetPositionByBounds(targetPoints[index]);

        if (targetObject != null)
        {
            if (selection != null)
                selection.RemoveAsSpawnOrTarget();
            else
                targetObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }

        targetObject = targetPoints[index];
        SetMarker(targetObject);

        selection = targetObject.GetComponent<Selectable>();
        if (selection != null) // Case when object chosen has Selectable component
        {
            selection.SetAsSpawnOrTarget(targetHighlightMaterial);
        }
        else // Necessary actions to replicate Selectable Selected options without Selectable component
        {
            MeshRenderer currentModelRenderer = targetObject.GetComponent<MeshRenderer>();
            originalMaterial = currentModelRenderer.material;
            currentModelRenderer.material = targetHighlightMaterial;
        }
    }

    private void SetMarker(GameObject markedObject)
    {
        Vector3 markerAddition = new Vector3(0.0f, markerHeight, 0.0f);
        Vector3 objectPosition;

        Renderer rend = markedObject.GetComponent<Renderer>();
        if (rend != null)
            objectPosition = rend.bounds.center;
        else
            objectPosition = markedObject.transform.position;
        
        if (!targetMarker.activeInHierarchy)
            targetMarker.SetActive(true);

        targetMarker.transform.position = objectPosition + markerAddition;
    }

    public void AddNewOption(Transform selection)
    {
        GlobalModelData.Instance.AddToTargetObjects(selection.gameObject);

        AddToDropdownList(selection.gameObject.name);
    }
}
