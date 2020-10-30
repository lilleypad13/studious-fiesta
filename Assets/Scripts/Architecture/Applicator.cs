using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Applicator
{
    [SerializeField] private string searchTerm;
    public string SearchTerm 
    { 
        get => searchTerm; 
        set { searchTerm = value; } 
    }
}
