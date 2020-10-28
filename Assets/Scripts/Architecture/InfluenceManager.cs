using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceManager : MonoBehaviour
{
    #region Variables
    private Influence[] influenceObjects;
    [SerializeField]private AGridRuntime aGridRuntime;

    #endregion

    #region Unity Methods

    /*
     * Cycles through all of the existing architectural influence objects given to it and uses their 
     * individual ApplyInfluence methods to spread values from the influencing objects onto the
     * node grid passed in.
     */
    public void ApplyInfluence(Node[,] grid)
    {
        foreach (Influence influencer in influenceObjects)
        {
            //Node influencerOrigin = aGridRuntime.NodeFromWorldPoint(influencer.transform.position);
            Node influencerOrigin = aGridRuntime.TestNodeFromWorldPoint(transform.TransformPoint(influencer.transform.position));

            influencer.ApplyInfluence(grid, influencerOrigin);
        }
    }

    /*
     * Search the entire scene for all the Influence objects and collect them in influenceObjects
     */
    public void FindInfluenceObjects()
    {
        influenceObjects = FindObjectsOfType<Influence>();
    }
    #endregion
}
