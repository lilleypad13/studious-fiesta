using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevitAddInfluenceDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string data)
    {
        if (!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
            Influence.AddInfluence(modelToModify, data, 20, 20, 46);
        //else
        //    Debug.Log("Handler tried to add Influence component with empty or null name.");
    }
}
