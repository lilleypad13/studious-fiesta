using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSpawnManager : DropdownManager
{
    [SerializeField] private AgentSpawnManager spawnManager;
    [SerializeField] private Material spawnHighlightMaterial;
    [SerializeField] GameObject spawnMarker;
    [SerializeField] float markerHeight = 10.0f;

    List<GameObject> spawnPoints = new List<GameObject>();
    private GameObject spawnObject;

    private Material originalMaterial;
    private Selectable selection;

    protected override void Start()
    {
        base.Start();

        if (spawnManager.AllObjectsAsSpawnTargets)
            spawnPoints = GlobalModelData.Instance.ObjectsInEntireModel;
        else
            spawnPoints = GlobalModelData.Instance.spawnObjects;

        foreach (GameObject item in spawnPoints)
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
        spawnManager.SpawnPosition = GlobalModelData.Instance.GetPositionByBounds(spawnPoints[index]);

        if(spawnObject != null)
        {
            if (selection != null)
                selection.RemoveAsSpawnOrTarget();
            else
                spawnObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }

        spawnObject = spawnPoints[index];
        SetMarker(spawnObject);

        selection = spawnObject.GetComponent<Selectable>();
        if(selection != null) // Case when object chosen has Selectable component
        {
            selection.SetAsSpawnOrTarget(spawnHighlightMaterial);
        }
        else // Necessary actions to replicate Selectable Selected options without Selectable component
        {
            MeshRenderer currentModelRenderer = spawnObject.GetComponent<MeshRenderer>();
            originalMaterial = currentModelRenderer.material;
            currentModelRenderer.material = spawnHighlightMaterial;
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

        if (!spawnMarker.activeInHierarchy)
            spawnMarker.SetActive(true);

        spawnMarker.transform.position = objectPosition + markerAddition;
    }

    public void AddNewOption(Transform selection)
    {
        GlobalModelData.Instance.AddToSpawnObjects(selection.gameObject);

        AddToDropdownList(selection.gameObject.name);
    }
}
