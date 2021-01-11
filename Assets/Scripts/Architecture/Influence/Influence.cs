using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Influence : MonoBehaviour
{
    #region Variables
    public string InfluenceName 
    { 
        get => influenceName;
        set
        {
            GlobalModelData.Instance.AddIfNotInDictionary(value);
            influenceName = value;
        }
    }
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
    public static void AddInfluence(GameObject objectToAddInfluenceTo, string _influenceName, int _rangeX, int _rangeZ, int _influenceValue)
    {
        if(!string.IsNullOrEmpty(_influenceName) && !string.IsNullOrWhiteSpace(_influenceName))
        {
            Influence influencer = objectToAddInfluenceTo.AddComponent<Influence>();
            influencer.InfluenceName = _influenceName;
            influencer.xInfluence = _rangeX;
            influencer.zInfluence = _rangeZ;
            influencer.ArchitectureInfluenceAmount = _influenceValue;
        }
    }

    private void Awake()
    {
        rend = this.gameObject.GetComponent<Renderer>();
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
    public void ApplyInfluence(Node[,] grid)
    {
        rend = this.gameObject.GetComponent<Renderer>();
        influenceOriginPosition = DetermineInfluenceOriginPosition();
        Node influenceOrigin = AGridRuntime.Instance.NodeFromWorldPoint(influenceOriginPosition);

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

        Debug.Log($"{this.gameObject.name} applied influence around node {influenceOrigin.NodeCoordinates}");
    }

    #endregion
}
