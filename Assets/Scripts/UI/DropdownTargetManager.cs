using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownTargetManager : DropdownManager
{
    [SerializeField] private AgentSpawnManager spawnManager;
    [SerializeField] private Text textBox;
    [SerializeField] private Material targetHighlightMaterial;

    List<GameObject> spawnPoints = new List<GameObject>();

    private Material originalMaterial;
    private GameObject currentModel;

    protected override void Start()
    {
        base.Start();

        spawnPoints = GlobalModelData.Instance.ObjectsInEntireModel;

        foreach (GameObject item in spawnPoints)
        {
            AddToDropdownList(item.name);
        }
    }


    protected override void DropdownItemSelected(Dropdown dropdown)
    {
        base.DropdownItemSelected(dropdown);
        textBox.text = "Current Target: " + DropdownText(dropdown.value);
    }


    protected override void MethodToPerformOnSelection(int index)
    {
        spawnManager.Target = spawnPoints[index].transform;

        if (currentModel != null)
        {
            currentModel.GetComponent<MeshRenderer>().material = originalMaterial;
        }
        currentModel = spawnPoints[index];
        MeshRenderer currentModelRenderer = currentModel.GetComponent<MeshRenderer>();

        originalMaterial = currentModelRenderer.material;
        currentModelRenderer.material = targetHighlightMaterial;
    }
}
