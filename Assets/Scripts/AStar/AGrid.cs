using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycastAGridDetermination))]
public class AGrid : Initializer
{
    [Header("Node and Grid Dimensions")]
    private Vector2 gridWorldSize; // area in world coordinates that grid will cover
    [Tooltip("EXPERIMENTAL: Normaly only works with value of 1")]
    [SerializeField] private float nodeDiameter = 1.0f; // how much space each individual node covers
    public float NodeDiameter { get => nodeDiameter; }
    public float unitHeight = 0.0f; // Used to adjust node positioning for agent height

    [Header("Node Grid Options")]
    public bool isReadingDataFromFile = false;
    public bool useHalfNodeOffset = true;

    //[Header("Obstacle Parameters")]
    private int obstacleProximityPenalty = 10;

    [Header("Unity Editor Data Visualization")]
    public bool displayGridGizmos;

    private Node[,] grid; //2D array of nodes
    public NodeGrid nodeGrid; // Helpful package of data about grid
    private int gridSizeX, gridSizeY;

    private AGridRuntime aGridRuntime;
    private InfluenceManager influenceManager;
    private AGridDataVisualization dataVisualizer;
    private RaycastAGridDetermination raycastSystem;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    public override void Initialization()
    {
        aGridRuntime = AGridRuntime.Instance;
        influenceManager = GetComponent<InfluenceManager>();
        dataVisualizer = GetComponent<AGridDataVisualization>();
        raycastSystem = GetComponent<RaycastAGridDetermination>();

        gridWorldSize.x = GlobalModelData.Instance.ModelBounds.size.x;
        gridWorldSize.y = GlobalModelData.Instance.ModelBounds.size.z;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        Debug.Log($"Grid World Sizes are: x: {gridWorldSize.x}; z: {gridWorldSize.y}. \n" +
            $"Grid Index Sizes are: x: {gridSizeX}; z: {gridSizeY}.");

        if (isReadingDataFromFile)
            CSVReaderRevitDataToAStarGrid.Instance.ReadInData(); // Ensures data is read in from .csv file before trying to assign values from it

        influenceManager.FindInfluenceObjects(); // Automatically finds all influence objects in scene
        CreateGrid();

        //DebugNodeGridData();
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
        Vector3 worldBottomLeft = new Vector3();
        worldBottomLeft = GlobalModelData.Instance.ModelBounds.min;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Determines the actual position of the node
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
                walkable = raycastSystem.DetermineWalkabilityWithRaycast(worldPoint);

                if (!walkable)
                    movementPenalty += obstacleProximityPenalty;

                // Create node and assign determined values
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

                if (isReadingDataFromFile)
                    ModifyDataForPathingNodes.Instance.CheckToAssignValue(grid[x, y]);
            }
        }

        ModifyDataForPathingNodes.Instance.DebugListNodesThatWereUnassigned();

        // Used to pass on constructed node grid information to AGridRuntime to deal with run time events pertaining to grid
        // TODO: MUST REMOVE DOUBLE GRID SETTING HERE; NEEDED CURRENTLY IN ORDER TO APPLY INFLUENCE
        nodeGrid = new NodeGrid(grid, gridWorldSize, gridSizeX, gridSizeY);
        aGridRuntime.SetGrid(nodeGrid);

        influenceManager.ApplyInfluence(grid);

        // Used to pass on constructed node grid information to AGridRuntime to deal with run time events pertaining to grid
        nodeGrid = new NodeGrid(grid, gridWorldSize, gridSizeX, gridSizeY);
        aGridRuntime.SetGrid(nodeGrid);
    }

    #region Data Visualization

    // Displays the nodes in a more visual way while also being able to convey information about the nodes
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && displayGridGizmos)
        {
            dataVisualizer.VisualizeNodeData(nodeDiameter / 2.0f, gridSizeX, gridSizeY, grid);
        }

    }
    #endregion

    #region Debugging
    private void DebugCheckObjectRaycastHitAtLocation(Vector3 worldPoint, RaycastHit hit)
    {
        if (hit.collider.gameObject.layer != 0)
            Debug.Log($"Raycast at {worldPoint} hit {hit.collider.name}");
    }

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

    private void DebugNodeGridData()
    {
        string message = "Entire node grid data: \n";

        foreach (Node node in grid)
        {
            message += $"Node Coordinates {node.gridX}, {node.gridY}; World Position: {node.worldPosition} \n";
        }

        Debug.Log(message);
    }
    #endregion
}
