using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

[Serializable]
public class InfluenceApplicator : Applicator
{
    [Inherits(typeof(Influence))]
    [SerializeField] private TypeReference influenceComponentToApply;
    public TypeReference InfluenceComponentToApply
    {
        get => influenceComponentToApply;
        set => influenceComponentToApply = value;
    }

    [SerializeField] private int influenceAmount;
    public int InfluenceAmount
    {
        get => influenceAmount;
        set
        {
            if (value >= MathArchCost.Instance.MIN_ARCHVALUE)
            {
                if (value <= MathArchCost.Instance.MAX_ARCHVALUE)
                    influenceAmount = value;
                else
                    influenceAmount = MathArchCost.Instance.MAX_ARCHVALUE;
            }
            else
                influenceAmount = MathArchCost.Instance.MIN_ARCHVALUE;
        }
    }
}
