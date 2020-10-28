using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    [Header("Node and Grid Dimensions")]
    public Vector2 gridWorldSize; // area in world coordinates that grid will cover
    [Tooltip("EXPERIMENTAL: Normaly only works with value of 1")]
    public float nodeDiameter = 1.0f; // how much space each individual node covers
    public float unitHeight = 0.0f; // Used to adjust node positioning for agent height

    [Header("Node Grid Options")]
    public bool gridOriginAtBottomLeft = false;
    public bool isReadingDataFromFile = false;
    public bool useHalfNodeOffset = true;

    [Header("Layer Determination")]
    public LayerMask unwalkableMask;
    public TerrainType[] walkableRegions;

    //[Header("Obstacle Parameters")]
    private int obstacleProximityPenalty = 10;

    [Header("Unity Editor Data Visualization")]
    public bool displayGridGizmos;

    private LayerMask walkableMask;
    private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    private Node[,] grid; //2D array of nodes
    public NodeGrid nodeGrid; // Helpful package of data about grid

    //private float nodeDiamater;
    private int gridSizeX, gridSizeY;

    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MinValue;

    private AGridRuntime aGridRuntime;
    private InfluenceManager influenceManager;
    private AGridDataVisualization dataVisualizer;

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }

    // Determines how many nodes can be fit into the grid based on the overall grid size 
    // and the size of the individual nodes
    private void Awake()
    {
        aGridRuntime = GetComponent<AGridRuntime>();
        influenceManager = GetComponent<InfluenceManager>();
        dataVisualizer = GetComponent<AGridDataVisualization>();

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        if(isReadingDataFromFile)
            CSVReader.Instance.ReadInData(); // Ensures data is read in from .csv file before trying to assign values from it

        influenceManager.FindInfluenceObjects(); // Automatically finds all influence objects in scene
        CreateGrid();
    }

    // Returns the overall area of the grid given its dimensions
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    /*
     * Creates the overall grid of nodes to work with for the A* system
     * Generates a 2D array of nodes based on gridSizeX and gridSizeY, positions these nodes 
     * based on the nodeDiameter and overall gridWorldSize, then assigns base values to these 
     * nodes (such as movement penalty costs and whether they are traversable or not in the first 
     * place).
     */
    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // Creates a 2D grid that gives the total number of nodes

        // This point represents the bottom left corner of the grid
        // Designer bool can determine whether Unity's origin matches the center of the grid or the bottom left point of the grid
        Vector3 worldBottomLeft = new Vector3();
        if (gridOriginAtBottomLeft)
            worldBottomLeft = Vector3.zero;
        else
            worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Determines the actual position of the node
                //Vector3 worldPoint = worldBottomLeft +
                //    Vector3.right * (x * nodeDiameter + nodeDiameter / 2.0f) +
                //    Vector3.forward * (y * nodeDiameter + nodeDiameter / 2.0f);
                Vector3 worldPoint = worldBottomLeft +
                    Vector3.right * ((x + 1) * nodeDiameter) +
                    Vector3.forward * ((y + 1) * nodeDiameter);
                if (useHalfNodeOffset) // Centers nodes based on half their dimensions
                    worldPoint -= Vector3.right * (nodeDiameter / 2.0f) + Vector3.forward * (nodeDiameter / 2.0f);


                // Determine if node is traversable
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                bool walkable = true;

                int movementPenalty = 0;

                // Raycast
                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.layer == 8)
                        walkable = false;
                    else
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        worldPoint.y = hit.point.y + unitHeight / 2;
                    }
                }
                else
                    walkable = false;

                if (!walkable)
                    movementPenalty += obstacleProximityPenalty;

                // Create node and assign determined values
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                if (isReadingDataFromFile)
                    CSVReader.Instance.CheckToAssignValue(grid[x, y]); // Determines if value from CSV file should be applied to node
            }
        }

        // Used to pass on constructed node grid information to AGridRuntime to deal with run time events pertaining to grid
        // TODO: MUST REMOVE DOUBLE GRID SETTING HERE; NEEDED CURRENTLY IN ORDER TO APPLY INFLUENCE
        nodeGrid = new NodeGrid(grid, gridWorldSize, gridSizeX, gridSizeY);
        aGridRuntime.SetGrid(nodeGrid);

        influenceManager.ApplyInfluence(grid);

        // Blur the penalty map to reduce "hard penalty boundaries"; Softens change in penalty between areas
        //BlurPenaltyMap(3);

        // Used to pass on constructed node grid information to AGridRuntime to deal with run time events pertaining to grid
        nodeGrid = new NodeGrid(grid, gridWorldSize, gridSizeX, gridSizeY);
        aGridRuntime.SetGrid(nodeGrid);
    }

    /*
     * Blends penalty values throughout the map together so there are less discrete sections for areas with different 
     * penalty values.
     */
    private void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        //int kernelExtents = (kernelSize - 1) / 2;
        int kernelExtents = blurSize;

        // Creates two new grids of equal size to the original grid to hold the updated blurred values.
        // The horizontal pass uses values from the original grid to fill its own first, 
        // then the vertical pass uses the values from the horizontal grid to fill itself after.
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        // All of the Mathf.Clamp methods used are to effectively reproduce values at the boundaries of the grid 
        // to keep anything from going out of the grid bounds upon calculation.
        for (int y = 0; y < gridSizeY; y++)
        {
            // Fills the first element in the row based on the kernel size
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            // Fills the rest of the elements after the first
            // As this process moves throughout the grid, it starts with the last value calculated in the grid and subtracts 
            // the leftmost elements from that value (that were dropped from the kernel) and adds the new rightmost elements 
            // (newly added to the kernel upon advancing) to obtain the value for the current grid location.
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                // Generates the next value by simply subtracting the value from the single element 
                // removed from the kernel and adding the value from the newly added element
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] 
                    - grid[removeIndex, y].movementPenalty
                    + grid[addIndex, y].movementPenalty;
            }
        }

        // Same process, but reverse x and y for vertical process
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1]
                    - penaltiesHorizontalPass[x, removeIndex]
                    + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                // Solely for visualization purposes
                if(blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;
                if(blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }
    }

    #region Debugging and Data Visualization

    /*
     * For debugging grid; Lists all the architectural information of every node in the grid
     */
    public void ListArchitecturalParametersOfGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y].ArchitecturalOutput();
            }
        }
    }

    // Displays the nodes in a more visual way while also being able to convey information about the nodes
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && displayGridGizmos)
        {
            dataVisualizer.VisualizeNodeData(nodeDiameter / 2.0f, gridSizeX, gridSizeY, grid);
        }

    }

    #endregion
}
