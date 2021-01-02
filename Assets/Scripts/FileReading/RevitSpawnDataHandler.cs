using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevitSpawnDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string data)
    {
        if(!string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
        {
            GlobalModelData.Instance.AddToSpawnObjects(modelToModify);
            GlobalModelData.Instance.AddToTargetObjects(modelToModify);
        }
    }
}
