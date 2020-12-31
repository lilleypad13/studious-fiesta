using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArchitecturalElement
{
    public readonly string Name;

    public int ArchitecturalValue
    {
        get => architecturalValue;
        set
        {
            architecturalValue = value;
            CheckAgainstGlobalArchitecturalMinAndMax();
        }
    }
    private int architecturalValue = 0;


    public ArchitecturalElement(string _name)
    {
        Name = _name;
        GlobalModelData.Instance.AddIfNotInDictionary(_name);
    }

    public ArchitecturalElement(string _name, int _value)
    {
        Name = _name;
        GlobalModelData.Instance.AddIfNotInDictionary(_name);

        ArchitecturalValue = _value;
    }

    private void CheckAgainstGlobalArchitecturalMinAndMax()
    {
        GlobalModelData.Instance.CheckValueAgainstArchitecturalElementContainer(this);
    }
}
