using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    [SerializeField]private Initializer[] initializingClasses;

    private void Awake()
    {
        foreach (Initializer initializer in initializingClasses)
        {
            initializer.Initialization();
            DebugCurrentInitializationStep(initializer);
        }
    }

    #region Debug Methods

    private int initializationStep = 0;

    public void DebugCurrentInitializationStep(Initializer currentInitializer)
    {
        Debug.Log($"Step {initializationStep}: {currentInitializer.GetType()} initialized.");
        initializationStep++;
    }

    #endregion
}
