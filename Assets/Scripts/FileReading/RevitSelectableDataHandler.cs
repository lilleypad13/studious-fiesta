using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevitSelectableDataHandler : IRevitModelDataHandler
{
    private string selectableCheck = "No";

    public void ModifyModelWithData(GameObject modelToModify, string check)
    {
        if (!string.IsNullOrEmpty(check) || !string.IsNullOrWhiteSpace(check))
        {
            GameObject.Destroy(modelToModify.GetComponent<Selectable>());
        }
    }
}
