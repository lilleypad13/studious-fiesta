using UnityEngine;

public class RevitWalkabilityDataHandler : IRevitModelDataHandler
{
    public void ModifyModelWithData(GameObject modelToModify, string isUnwalkable)
    {
        int unwalkableCheck = 0;
        int.TryParse(isUnwalkable, out unwalkableCheck);

        CheckWalkability(modelToModify, unwalkableCheck);

        foreach (Transform child in modelToModify.transform)
        {
            CheckWalkability(child.gameObject, unwalkableCheck);
        }
    }

    private void CheckWalkability(GameObject objectChecked, int unwalkableCheck)
    {
        if (unwalkableCheck != 1)
            objectChecked.layer = LayerMask.NameToLayer("Default");
        else
            objectChecked.layer = LayerMask.NameToLayer("Unwalkable");
    }
}
