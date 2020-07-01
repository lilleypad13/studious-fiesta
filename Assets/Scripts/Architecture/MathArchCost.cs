using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathArchCost : MonoBehaviour
{
    /*
     * Contains all the math and constraints for agent affinity values and architecture values within nodes.
     * Math for converting these values and their interactions into flat costs to incorporate in 
     * general A* pathfinding logic.
     */
    private static MathArchCost instance;
    private MathArchCost()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static MathArchCost Instance
    {
        get
        {
            if (instance == null)
            {
                new MathArchCost();
            }
            return instance;
        }
    }

    // Affinity Parameters
    public int AVERAGE_AFFINITY // A
    {
        get { return _AVERAGE_AFFINITY; }
    }
    private int _AVERAGE_AFFINITY = 50;

    public int MAX_AFFINITY // 2A
    {
        get { return 2 * _AVERAGE_AFFINITY; }
    }

    // Architecture Value Parameters
    public int MIN_ARCHVALUE
    {
        get { return _MIN_ARCHVALUE; }
    }
    [SerializeField]private int _MIN_ARCHVALUE = 0;

    public int MAX_ARCHVALUE // B
    {
        get { return _MAX_ARCHVALUE; }
    }
    [SerializeField]private int _MAX_ARCHVALUE = 100;

    public int ARCHCOST_DEFAULT // D
    {
        get { return _ARCHCOST_DEFAULT; }
    }
    [SerializeField]private int _ARCHCOST_DEFAULT = 1000;

    private int MIN_COSTPERARCH // pmin = -D/B because I want [pmin * B = D] to cancel it in the end
    {
        get { return (-ARCHCOST_DEFAULT / MAX_ARCHVALUE); }
    }
    private int MAX_COSTPERARCH // pmax = -pmin
    {
        get { return -MIN_COSTPERARCH; }
    }

    // Individual Architectural Parameter Controllers
    [SerializeField]private int minConnectivity = 0;
    [SerializeField]private int maxConnectivity = 3500;

    public int normalizeConnectivity(int connectivity)
    {
        int normalizedValue = 0;
        int connectivityRange = maxConnectivity - minConnectivity;

        normalizedValue = (int)((float)(connectivity - minConnectivity) / connectivityRange * _MAX_ARCHVALUE);

        return normalizedValue;
    }
    

    // Use a to calculate p
    public int CalculateCostPerArchFromAffinity(int affinity)
    {
        int costPerArchValue = 0; // p

        // Using simple linear relationship: y = mx + b
        // converted to: p = ma + pmax
        // p = cost/architecture value; a = affinty
        // pmin = cost/architecture value's lowest value (min comes from the highest affinity value)
        // m = slope which gives pmin at max affinity and pmax at min affinity

        // CAUTION: int math could produce strange rounding errors, so check those if numbers do not add
        // up properly

        // m = (pmin - pmax)/2A
        float slope = (MIN_COSTPERARCH - MAX_COSTPERARCH) / (float)MAX_AFFINITY;

        // p = ma + pmax
        costPerArchValue = (int)(slope * affinity + MAX_COSTPERARCH); // Needs cast as int after using slope as float

        //Debug.Log("MathArchCalcs calculates: \n" +
        //    " Slope = " + slope + "\n" +
        //    " CostPerArchValue = " + costPerArchValue);

        return costPerArchValue; // p
    }

    public void ValuesCheck()
    {
        Debug.Log(
            "Average Affinity: " + AVERAGE_AFFINITY + "\n" +
            "Max Affinity: " + MAX_AFFINITY + "\n" +
            "Min Architecture Value: " + MIN_ARCHVALUE + "\n" +
            "Max Architecture Value: " + MAX_ARCHVALUE + "\n" +
            "Default Architecture Cost: " + ARCHCOST_DEFAULT + "\n" +
            "Min Architecture Cost per Affinity: " + MIN_COSTPERARCH + "\n" +
            "Max Architecture Cost per Affinity: " + MAX_COSTPERARCH);
    }
}
