using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    protected Dropdown dropdown;

    protected virtual void Start()
    {
        dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "None" });

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    protected virtual void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        // Method to perform when selecting dropdown item
        index -= 1; // -1 to index to account for extra dropdown "None" option
        if (index >= 0)
            MethodToPerformOnSelection(index);
    }


    protected virtual void MethodToPerformOnSelection(int index)
    {
        Debug.LogWarning("Dropdown selection method not set to anything.");
    }

    protected void AddToDropdownList(string item)
    {
        dropdown.options.Add(new Dropdown.OptionData() { text = item });
    }


    protected string DropdownText(int index)
    {
        return dropdown.options[index].text;
    }
}
