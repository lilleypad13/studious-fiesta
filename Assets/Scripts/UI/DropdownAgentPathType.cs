using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownAgentPathType : MonoBehaviour
{
    //public Text textBox;

    public PathfindingHeapSimple pathingController;
    private string[] architectureTypes;

    private Dropdown dropdown;
    private int pathCounter;

    private void Start()
    {
        dropdown = transform.GetComponent<Dropdown>();

        dropdown.options.Clear();

        // Fill dropdown with items
        // This populates the dropdown with the strings found in items
        architectureTypes = System.Enum.GetNames(typeof(ArchitecturePathingData));
        for (int i = 0; i < architectureTypes.Length; i++)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = architectureTypes[i] });
        }

        // Adds the method DropdownItemSelected with this parameter to the onValueChanged delegate
        // Makes it so that when onValueChanged Unity event is invoked with this dropdown, 
        // DropdownItemSelected is also called
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    /*
     * Actions to perform when the dropdown value changes (a dropdown item is selected)
     */
    private void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        pathingController.SetArchitecturalType(index); // Index of dropdown should matchup with enum index because dropdown list is created by enum order initially
    }

    /*
     * Add an element to this dropdown from an outside source while supplying the string 
     * to be used as the text for that new dropdown element.
     */
    public void AddToDropdownList(string item)
    {
        dropdown.options.Add(new Dropdown.OptionData() { text = item });
    }
}
