using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathHistoryVisualizer : MonoBehaviour
{
    private DataRecorder dataRecorder;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        dataRecorder = DataRecorder.Instance;
        lineRenderer = GetComponent<LineRenderer>();
    }

    /*
     * Allows outside sources to deactivate the line renderer
     */
    public void DeactivatePathVisual()
    {
        lineRenderer.enabled = false;
    }

    /*
     * Informs LineRenderer how to draw a line between all the different node's world positions within 
     * an entire path obtained from the DataRecorder pathRecords.
     */
    public void VisualizePath(int index)
    {
        PathData pathData = dataRecorder.GetPathRecord(index);

        if (pathData == null)
            Debug.Log("Path Visualizer failed to obtain pathData");

        Node[] path = pathData.GetNodePath();
        List<Vector3> pathPositions = new List<Vector3>();

        if(path == null)
            Debug.Log("Path Visualizer failed to obtain path");

        foreach (Node node in path)
        {
            pathPositions.Add(node.worldPosition);
        }

        Vector3[] pathPositionsArray = pathPositions.ToArray();

        lineRenderer.positionCount = pathPositionsArray.Length;
        lineRenderer.SetPositions(pathPositionsArray);

        lineRenderer.enabled = true;
    }
}
