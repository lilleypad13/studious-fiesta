using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownPathHistory : MonoBehaviour
{
    public Text textBox;

    public PathHistoryVisualizer pathVisualizer;

    private Dropdown dropdown;
    private int pathCounter;

    private void Start()
    {
        dropdown = transform.GetComponent<Dropdown>();

        pathCounter = 0;
        dropdown.options.Clear();

        //List<string> items = new List<string>();
        //items.Add("Item 1");
        //items.Add("Items 2");

        //// Fill dropdown with items
        //// This populates the dropdown with the strings found in items
        //foreach (string item in items)
        //{
        //    dropdown.options.Add(new Dropdown.OptionData() { text = item });
        //}

        //DropdownItemSelected(dropdown);

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
        // ADD VISUALIZATION INITIATION HERE
        // Using the dropdown index, recall path data for the equivalent index in the path data history

        int index = dropdown.value;
        Debug.Log("Dropdown index is: " + index);

        textBox.text = dropdown.options[index].text;
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
