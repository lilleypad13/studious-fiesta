using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHistoryVisualizer : MonoBehaviour
{
    private DataRecorder dataRecorder;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        //dataRecorder = GetComponent<DataRecorder>();
        dataRecorder = DataRecorder.Instance;
        lineRenderer = GetComponent<LineRenderer>();

        Debug.Log("PathHistoryVisualizer got DataRecorder reference: " + dataRecorder.gameObject.name);
        Debug.Log("PathHistoryVisuzlier got Linerenderer reference: " + lineRenderer.name);
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
    }
}
