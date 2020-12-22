using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DataVisual
{
    walkable, 
    window, 
    connectivity
}

public class AGridDataVisualization : MonoBehaviour
{
    private AGrid aGrid;
    [SerializeField]private DataVisual dataToVisualize = DataVisual.walkable;
    public Text dataVisualizedText;

    private void Awake()
    {
        aGrid = GetComponent<AGrid>();
    }

    private void Update()
    {
        dataVisualizedText.text = "Current Data Visualized: " + dataToVisualize.ToString();
    }

    // Displays the nodes in a more visual way while also being able to convey information about the nodes
    public void VisualizeNodeData(float nodeRadius, int gridSizeX, int gridSizeY, Node[,] grid)
    {
        Vector3 nodePosition = new Vector3();
        Vector3 pointSize = new Vector3(nodeRadius, nodeRadius, nodeRadius);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                switch (dataToVisualize)
                {
                    case DataVisual.walkable:
                        WalkableNodeCheck(grid[x, y], pointSize);
                        break;
                    case DataVisual.window:
                        nodePosition = grid[x, y].worldPosition;
                        Gizmos.color = HeatMapColor(MathArchCost.Instance.MIN_ARCHVALUE, MathArchCost.Instance.MAX_ARCHVALUE, grid[x, y].Window);
                        Gizmos.DrawCube(nodePosition, pointSize);
                        break;
                    case DataVisual.connectivity:
                        nodePosition = grid[x, y].worldPosition;
                        Gizmos.color = HeatMapColor(MathArchCost.Instance.MinConnectivity, MathArchCost.Instance.MaxConnectivity, grid[x, y].Connectivity);
                        Gizmos.DrawCube(nodePosition, pointSize);
                        break;
                    default:
                        nodePosition = grid[x, y].worldPosition;
                        Gizmos.color = new Color(x/gridSizeX, x/gridSizeX, x/gridSizeX) ;
                        Gizmos.DrawCube(nodePosition, pointSize);
                        break;
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
