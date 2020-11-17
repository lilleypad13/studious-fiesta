using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModelBoundsFinder
{
    public static Bounds EncapsulateGroupOfModelRenderers(List<GameObject> groupOfModels)
    {
        Bounds combinedBounds = groupOfModels[0].GetComponent<Renderer>().bounds;

        foreach (GameObject model in groupOfModels)
        {
            combinedBounds.Encapsulate(model.GetComponent<Renderer>().bounds);
        }

        return combinedBounds;
    }

    public static string ReportModelBounds(Bounds modelBounds)
    {
        Bounds bounds = modelBounds;

        string boundsMessage = "===Model Bounds Report===\n";

        boundsMessage += $"Center: {bounds.center}\n";
        boundsMessage += $"Extents: {bounds.extents}\n";
        boundsMessage += $"Max: {bounds.max}\n";
        boundsMessage += $"Min: {bounds.min}\n";
        boundsMessage += $"Size: {bounds.size}\n";
        boundsMessage += $"X: {bounds.size.x}\n";
        boundsMessage += $"Y: {bounds.size.y}\n";
        boundsMessage += $"Z: {bounds.size.z}\n";

        return boundsMessage;
    }

    public static void ReportListOfBounds(List<GameObject> modelList)
    {
        Renderer rendererToCheck;
        foreach (GameObject model in modelList)
        {
            rendererToCheck = model.GetComponent<Renderer>();
            if (rendererToCheck != null)
                Debug.Log($"{model.name} Bounds Report:\n" + ReportModelBounds(rendererToCheck.bounds));
        }
    }

    #region Debugging
    public static void VisualizeBoundsBox(Bounds bounds)
    {
        OnDrawGizmos(bounds);
    }

    private static void OnDrawGizmos(Bounds bounds)
    {
        Gizmos.color = Color.yellow;
        if (bounds.size != Vector3.zero)
            Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
    #endregion
}
