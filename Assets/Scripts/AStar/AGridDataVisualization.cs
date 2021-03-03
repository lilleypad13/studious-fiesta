using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CubeDrawer))]
public class AGridDataVisualization : MonoBehaviour
{
    public CubeDrawer cubeDrawer;
    private string dataTypeVisualized;

    private float nodeRadius;
    private int gridSizeX;
    private int gridSizeY;
    private Node[,] grid;
    private bool isUsingGizmos;

    public void SetDataType(string dataType)
    {
        dataTypeVisualized = dataType;
        if(!isUsingGizmos)
            VisualizeNodeData();
    }

    public void SetVisualizer(float _nodeRadius, int _gridSizeX, int _gridSizeY, Node[,] _grid, bool _isUsingGizmos)
    {
        nodeRadius = _nodeRadius;
        gridSizeX = _gridSizeX;
        gridSizeY = _gridSizeY;
        grid = _grid;
        isUsingGizmos = _isUsingGizmos;
    }

    // Displays the nodes in a more visual way while also being able to convey information about the nodes
    public void VisualizeNodeData()
    {
        cubeDrawer.ResetCubeDrawer();

        if(!string.IsNullOrEmpty(dataTypeVisualized))
        {
            Vector3 nodePosition = new Vector3();
            Vector3 pointSize = new Vector3(nodeRadius, nodeRadius, nodeRadius);
            Color nodeColor = Color.black;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (string.Equals(dataTypeVisualized, "Walkable"))
                    {
                        nodePosition = grid[x, y].worldPosition;
                        if (grid[x, y].walkable)
                            nodeColor = Color.yellow;
                        else
                            nodeColor = Color.red;
                        //WalkableNodeCheck(grid[x, y], pointSize);
                    }
                    else if (GlobalModelData.architecturalElementContainers.ContainsKey(dataTypeVisualized))
                    {
                        nodePosition = grid[x, y].worldPosition;

                        string key = GlobalModelData.architecturalElementContainers[dataTypeVisualized].Name;

                        nodeColor = HeatMapColor(GlobalModelData.architecturalElementContainers[key].MinimumValue,
                            GlobalModelData.architecturalElementContainers[key].MaximumValue,
                            grid[x, y].architecturalElementTypes[key].ArchitecturalValue);
                    }

                    cubeDrawer.AddCubeToBatch(nodePosition, nodeColor);
                }
            }
        }
    }

    public void VisualizeNodeDataGizmos()
    {
        if (!string.IsNullOrEmpty(dataTypeVisualized))
        {
            Vector3 nodePosition = new Vector3();
            Vector3 pointSize = new Vector3(nodeRadius, nodeRadius, nodeRadius);

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (string.Equals(dataTypeVisualized, "Walkable"))
                    {
                        WalkableNodeCheck(grid[x, y], pointSize);
                    }
                    else if (GlobalModelData.architecturalElementContainers.ContainsKey(dataTypeVisualized))
                    {
                        nodePosition = grid[x, y].worldPosition;

                        string key = GlobalModelData.architecturalElementContainers[dataTypeVisualized].Name;

                        Gizmos.color = HeatMapColor(GlobalModelData.architecturalElementContainers[key].MinimumValue,
                            GlobalModelData.architecturalElementContainers[key].MaximumValue,
                            grid[x, y].architecturalElementTypes[key].ArchitecturalValue);

                        Gizmos.DrawCube(nodePosition, pointSize);
                    }
                    else
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(nodePosition, pointSize);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isUsingGizmos)
            VisualizeNodeDataGizmos();
    }

    private Color HeatMapColor(int min, int max, int architecturalValue)
    {
        Color heat = new Color();

        int range = max - min;
        float hue = architecturalValue / (float)range; // TODO: Find better coloring method
        if (architecturalValue == min)
            heat = Color.white;
        else
            heat = Color.HSVToRGB(hue, 1.0f, 1.0f);

        return heat;
    }

    /*
     * Used for debugging to check which nodes are being labeled walkable or unwalkable
     */
    private void WalkableNodeCheck(Node node, Vector3 displayCubeDimensions)
    {
        if (node.walkable)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawCube(node.worldPosition, displayCubeDimensions);
    }
}
