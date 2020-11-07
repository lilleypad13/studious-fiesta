using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCreator
{
    public void CreateColliders(List<GameObject> objectsToApplyCollidersTo)
    {
        foreach (GameObject item in objectsToApplyCollidersTo)
        {
            item.AddComponent<MeshCollider>();
            item.GetComponent<MeshCollider>().convex = true;
        }
    }
}
