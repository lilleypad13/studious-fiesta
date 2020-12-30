using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitecturalElementContainer
{
    public readonly string Name;

    public int MinimumValue { get => minimumValue; }
    private int minimumValue = 0;

    public int MaximumValue { get => maximumValue; }
    private int maximumValue = 0;

    public ArchitecturalElementContainer(string _name)
    {
        Name = _name;
    }

    public ArchitecturalElementContainer(string _name, int _initialValue)
    {
        Name = _name;
        CheckValueAgainstMinAndMax(_initialValue);
    }

    public void CheckValueAgainstMinAndMax(int valueToCheck)
    {
        if (valueToCheck > maximumValue)
            maximumValue = valueToCheck;
        else if (valueToCheck < minimumValue)
            minimumValue = valueToCheck;
    }
}
