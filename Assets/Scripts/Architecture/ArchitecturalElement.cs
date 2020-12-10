using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitecturalElement
{
    private string name;
    public string Name { get => name; }

    private int minimumValue = 0;
    public int MinimumValue
    {
        get => minimumValue;
        set
        {
            if (value < minimumValue)
                minimumValue = value;
        }
    }
    private int maximumValue = 0;
    public int MaximumValue
    {
        get => maximumValue;
        set
        {
            if (value > maximumValue)
                maximumValue = value;
        }
    }

    public ArchitecturalElement(string _name)
    {
        name = _name;
    }

    public void CheckValueAgainstMinMax(int valueToCheck)
    {
        if (minimumValue == 0 & maximumValue == 0)
            maximumValue = valueToCheck;
        else if (valueToCheck > maximumValue)
            maximumValue = valueToCheck;
        else if (valueToCheck < minimumValue)
            minimumValue = valueToCheck;
    }
}
