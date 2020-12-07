using UnityEngine;

public class RevitDoorDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string data)
    {
        int doorClosedCheck = 0;
        int.TryParse(data, out doorClosedCheck);

        if (doorClosedCheck == 1)
            modelToModify.layer = LayerMask.NameToLayer("Unwalkable");
        else
            modelToModify.layer = LayerMask.NameToLayer("Default");
    }
}
