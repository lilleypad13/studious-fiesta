using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Influence : MonoBehaviour
{
    #region Variables
    public string InfluenceName { get => influenceName; }
    [SerializeField]private string influenceName;

    public Renderer Rend { get => rend; set => rend = value; }
    private Renderer rend;

    public int xInfluence = 5;
    public int zInfluence = 5;

    public int ArchitectureInfluenceAmount{ get => architectureInfluenceAmount; set => architectureInfluenceAmount = value; }
    [SerializeField] private int architectureInfluenceAmount;


    public int xRange { get => xInfluence / 2; }
    public int zRange { get => zInfluence / 2; }

    public Vector3 InfluenceOriginPosition { get => influenceOriginPosition; set => influenceOriginPosition = value; }
    private Vector3 influenceOriginPosition;

    #endregion

    #region Unity Methods
    public Influence(string _name)
    {
        influenceName = _name;
        GlobalModelData.Instance.AddIfNotInDictionary(_name);
    }

    private void Awake()
    {
        if(InfluenceName != string.Empty)
            GlobalModelData.Instance.AddIfNotInDictionary(InfluenceName);

        Rend = this.gameObject.GetComponent<Renderer>();
        influenceOriginPosition = DetermineInfluenceOriginPosition();
    }

    private Vector3 DetermineInfluenceOriginPosition()
    {
        return rend.bounds.center;
    }

    /*
     * Takes in the overall A* grid and the origin of the influence object in terms of nodes 
     * so that individual Influence type object can determine which nodes to influence and how to 
     * influence them.
     * Node origin helps localize the node editing (so Influence class knows where to start influencing 
     * and where to spread out from)
     */
    public void ApplyInfluence(Node[,] grid, Node influenceOrigin)
    {
        for (int x = -xRange; x < xRange; x++)
        {
            for (int z = -zRange; z < zRange; z++)
            {
                if (AGridRuntime.Instance.WithinNodeGridBounds(influenceOrigin.gridX + x, influenceOrigin.gridY + z))
                {
                    if (grid[influenceOrigin.gridX + x, influenceOrigin.gridY + z].architecturalElementTypes.ContainsKey(influenceName))
                        grid[influenceOrigin.gridX + x, influenceOrigin.gridY + z].architecturalElementTypes[influenceName].ArchitecturalValue += architectureInfluenceAmount;
                }
            }
        }
    }


    /*
     * Debug to show which nodes an influence object is impacting on the grid.
     * influenceName can be manually put in to show which type of influence is being applied or the name of the object
     * applying influence.
     */
    public void DebugShowInfluencedNodes(string influenceName, int x, int z)
    {
        Debug.Log($"Applied {influenceName} to {x} , {z}.");
    }

    #endregion
}
