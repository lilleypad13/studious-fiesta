using UnityEngine;

public class RevitWallDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string data)
    {
        int wallWalkableCheck = 0;
        int.TryParse(data, out wallWalkableCheck);

        if (wallWalkableCheck != 1)
            modelToModify.layer = LayerMask.NameToLayer("Unwalkable");
        else
            modelToModify.layer = LayerMask.NameToLayer("Default");
    }
}
