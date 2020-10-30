using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

[Serializable]
public class ComponentApplicator : Applicator
{
    [Inherits(typeof(Influence))]
    [SerializeField] private TypeReference influenceComponentToApply;
    public TypeReference InfluenceComponentToApply
    {
        get => influenceComponentToApply;
        set { influenceComponentToApply = value; }
    }
}
