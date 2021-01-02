using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceManager : Initializer
{
    #region Variables
    private Influence[] influenceObjects;

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
            influencer.ApplyInfluence(grid);
        }
    }

    /*
     * Search the entire scene for all the Influence objects and collect them in influenceObjects
     */
    public void FindInfluenceObjects()
    {
        influenceObjects = FindObjectsOfType<Influence>();

        foreach (Influence influence in influenceObjects)
        {
            GlobalModelData.Instance.AddIfNotInDictionary(influence.InfluenceName);
        }
    }

    public override void Initialization()
    {
        FindInfluenceObjects();
    }
    #endregion
}
