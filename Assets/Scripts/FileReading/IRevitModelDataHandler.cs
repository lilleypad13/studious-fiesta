using UnityEngine;

interface IRevitModelDataHandler
{
    void ModifyModelWithData(GameObject modelToModify, string data);
}
