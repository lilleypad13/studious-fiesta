using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AGridDataVisualization : MonoBehaviour
{
    public string dataTypeVisualized = "Walkable";

    // Displays the nodes in a more visual way while also being able to convey information about the nodes
    public void VisualizeNodeData(float nodeRadius, int gridSizeX, int gridSizeY, Node[,] grid)
    {
        Vector3 nodePosition = new Vector3();
        Vector3 pointSize = new Vector3(nodeRadius, nodeRadius, nodeRadius);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (string.Equals(dataTypeVisualized, "Walkable"))
                    WalkableNodeCheck(grid[x, y], pointSize);
                else if(GlobalModelData.architecturalElementContainers.ContainsKey(dataTypeVisualized))
                {
                    nodePosition = grid[x, y].worldPosition;

                    string key = GlobalModelData.architecturalElementContainers[dataTypeVisualized].Name;

                    Gizmos.color = HeatMapColor(GlobalModelData.architecturalElementContainers[key].MinimumValue,
                        GlobalModelData.architecturalElementContainers[key].MaximumValue,
                        grid[x, y].architecturalElementTypes[key].ArchitecturalValue);

                    Gizmos.DrawCube(nodePosition, pointSize);
                }
            }
        }
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
