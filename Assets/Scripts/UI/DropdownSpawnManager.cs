using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSpawnManager : MonoBehaviour
{
    [SerializeField] private AgentSpawnManager spawnManager;
    List<GameObject> spawnPoints = new List<GameObject>();

    private Dropdown dropdown;
    [SerializeField] private Text textBox;

    private void Start()
    {
        spawnPoints = GlobalModelData.Instance.ObjectsInEntireModel;

        dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "None" });

        foreach (GameObject item in spawnPoints)
        {
            AddToDropdownList(item.name);
        }

        // Adds the method DropdownItemSelected with this parameter to the onValueChanged delegate
        // Makes it so that when onValueChanged Unity event is invoked with this dropdown, 
        // DropdownItemSelected is also called
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    
    private void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        textBox.text = dropdown.options[index].text;

        // Method to call in other class
        index -= 1; // -1 to index to account for extra dropdown "None" option
        if (index >= 0) 
            spawnManager.SpawnPoint = spawnPoints[index].transform;
    }

    public void AddToDropdownList(string item)
    {
        dropdown.options.Add(new Dropdown.OptionData() { text = item });
    }
}
