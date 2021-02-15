using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldToDropDownNameChanger : MonoBehaviour
{
    public DropdownManager[] dropdownManagers;
    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();
    }

    public void RenameDropdownItem(int managersArrayIndex)
    {
        if(!string.IsNullOrEmpty(inputField.text))
            dropdownManagers[managersArrayIndex].RenameCurrentDropDownItemText(inputField.text);
    }
}
