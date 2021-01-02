using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevitWalkabilityDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string isUnwalkable)
    {
        int unwalkableCheck = 0;
        int.TryParse(isUnwalkable, out unwalkableCheck);

        if (unwalkableCheck != 1)
            modelToModify.layer = LayerMask.NameToLayer("Default");
        else
            modelToModify.layer = LayerMask.NameToLayer("Unwalkable");
    }
}
