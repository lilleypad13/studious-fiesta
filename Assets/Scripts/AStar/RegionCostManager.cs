using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}

public class RegionCostManager : MonoBehaviour
{
    private LayerMask walkableMask;
    private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    [Header("Layer Determination")]
    public TerrainType[] walkableRegions;

    public void SetupRegionCosts()
    {
        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
    }

}
