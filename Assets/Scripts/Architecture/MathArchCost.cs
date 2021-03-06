﻿using System;
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
    public int MIN_AFFINITY { get => _MIN_AFFINITY; }
    [SerializeField]private int _MIN_AFFINITY = 0;

    //private int _AVERAGE_AFFINITY = 50;
    public int AVERAGE_AFFINITY { get => (_MAX_AFFINITY - _MIN_AFFINITY) / 2; } // A

    public int MAX_AFFINITY { get => _MAX_AFFINITY; } // 2A
    [SerializeField] private int _MAX_AFFINITY = 100;

    // Architecture Value Parameters
    public int MIN_ARCHVALUE{ get => _MIN_ARCHVALUE; }
    [SerializeField] private int _MIN_ARCHVALUE = 0;

    public int MAX_ARCHVALUE {get => _MAX_ARCHVALUE; } // B
    [SerializeField] private int _MAX_ARCHVALUE = 100;

    public int ARCHCOST_DEFAULT {get => _ARCHCOST_DEFAULT; } // D
    [SerializeField] private int _ARCHCOST_DEFAULT = 1000;

    // pmin = -D/B because I want [pmin * B = D] to cancel it in the end
    private int MIN_COSTPERARCH {get => -ARCHCOST_DEFAULT / MAX_ARCHVALUE; }
    // pmax = -pmin
    private int MAX_COSTPERARCH { get => -MIN_COSTPERARCH; }

    public int NormailzeArchitecturalValue(ArchitecturalElement element)
    {
        ArchitecturalElementContainer container = GlobalModelData.Instance.GetFromContainerDictionary(element.Name);
        int normalizedValue = 0;
        int elementRange = container.MaximumValue - container.MinimumValue;

        normalizedValue = (int)((float)(element.ArchitecturalValue - container.MinimumValue) / elementRange * _MAX_ARCHVALUE);

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
