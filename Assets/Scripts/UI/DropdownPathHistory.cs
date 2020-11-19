using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPathHistory : MonoBehaviour
{
    public Text textBox;

    [SerializeField] private PathHistoryVisualizer pathVisualizer;

    private Dropdown dropdown;
    private int pathCounter;

    private void Start()
    {
        dropdown = transform.GetComponent<Dropdown>();

        pathCounter = 1;
        dropdown.options.Clear();

        dropdown.options.Add(new Dropdown.OptionData() { text = "None" });

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
        pathVisualizer.DeactivatePathVisual(); // Turns off linerenderer for path visualization that comes back on if an actual new path is selected

        int index = dropdown.value;
        textBox.text = dropdown.options[index].text;

        index -= 1; // -1 to index to account for extra dropdown "None" option
        if (index >= 0) // Ensures no path is created for intial dropdown "None" option, while also preventing out of bounds checks
            pathVisualizer.VisualizePath(index);
    }

    /*
     * Add an element to this dropdown from an outside source while supplying the string 
     * to be used as the text for that new dropdown element.
     */
    public void AddToDropdownList(string item)
    {
        dropdown.options.Add(new Dropdown.OptionData() { text = item });
    }

    /*
     * Specifically used to add a new path to the dropdown list.
     * Keeps track of the number of paths added to number them as it populates 
     * the dropdown.
     */
    public void AddNewPathToDropdown()
    {
        string addedPathText = "Path " + pathCounter;
        AddToDropdownList(addedPathText);
        pathCounter++;
    }
}
