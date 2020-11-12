using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionChecker : MonoBehaviour
{
    private void Start()
    {
        DebugWorldPosition(this.gameObject);
        DebugLocalPosition(this.gameObject);
        DebugBoundsCenterPosition(this.gameObject);
    }

    public void DebugWorldPosition(GameObject objectToCheck)
    {
        Vector3 position = objectToCheck.transform.position;

        Debug.Log($"{objectToCheck.name} is located at world position {position}.");
    }

    public void DebugLocalPosition(GameObject objectToCheck)
    {
        Vector3 position = objectToCheck.transform.localPosition;

        Debug.Log($"{objectToCheck.name} is located at local position {position}.");
    }

    public void DebugBoundsCenterPosition(GameObject objectToCheck)
    {
        Renderer renderer;
        renderer = objectToCheck.GetComponent<Renderer>();

        if (renderer != null)
        {
            Vector3 position = renderer.bounds.center;

            Debug.Log($"{objectToCheck.name} has its bounds center at {position}.");
        }
        else
            Debug.Log($"{this.name} did not find a Renderer component on {objectToCheck.name}.");
    }
}
