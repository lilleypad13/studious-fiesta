using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class DropdownDataVisualization : DropdownManager
{
    private AGridDataVisualization dataVisualizer;

    protected override void Start()
    {
        base.Start();

        dataVisualizer = FindObjectOfType<AGridDataVisualization>();

        SetDataTypeOptions();
    }

    public void SetDataTypeOptions()
    {
        AddToDropdownList("Walkable");

        if (GlobalModelData.architecturalElementContainers.Count > 0)
        {
            foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
            {
                AddToDropdownList(container.Key);
            }
        }
    }


    protected override void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        if (index >= 0)
            MethodToPerformOnSelection(index);
    }


    protected override void MethodToPerformOnSelection(int index)
    {
        dataVisualizer.dataTypeVisualized = DropdownText(index);
    }
}
