using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownTargetManager : DropdownManager
{
    [SerializeField] private AgentSpawnManager spawnManager;
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
    }


    protected override void MethodToPerformOnSelection(int index)
    {
        spawnManager.Target = GlobalModelData.Instance.GetPositionByBounds(spawnPoints[index]);
        if (currentModel != null)
        {
            currentModel.GetComponent<MeshRenderer>().material = originalMaterial;
        }
        currentModel = spawnPoints[index];
        MeshRenderer currentModelRenderer = currentModel.GetComponent<MeshRenderer>();

        originalMaterial = currentModelRenderer.material;
        currentModelRenderer.material = targetHighlightMaterial;
        Debug.Log($"Target Highlight: \n Model: {currentModel.name} \n Original Material: {originalMaterial.name} \n Current Material: {currentModelRenderer.name}");
    }
}
