using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlobalModelData
{
    private static GlobalModelData instance = null;
    private static readonly object padlock = new object();

    public static GlobalModelData Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                    instance = new GlobalModelData();
            }
            return instance;
        }
    }

    private Bounds modelBounds;
    public Bounds ModelBounds
    {
        get => modelBounds;
        set => modelBounds = value;
    }
}
