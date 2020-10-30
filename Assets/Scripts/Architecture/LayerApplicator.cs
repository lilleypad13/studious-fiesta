using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LayerApplicator : Applicator
{
    [SerializeField] private LayerMask layerToApply;
    public LayerMask LayerToApply
    {
        get => layerToApply;
        set { layerToApply = value; }
    }
}
