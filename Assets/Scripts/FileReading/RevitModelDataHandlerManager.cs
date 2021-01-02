using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RevitModelDataHandlerManager
{
    private static RevitModelDataHandlerManager instance;
    private RevitModelDataHandlerManager()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static RevitModelDataHandlerManager Instance
    {
        get
        {
            if (instance == null)
            {
                new RevitModelDataHandlerManager();
            }
            return instance;
        }
    }

    private static RevitDoorDataHandler doorHandler = new RevitDoorDataHandler();
    private static RevitWallDataHandler wallHandler = new RevitWallDataHandler();
    private static RevitAddInfluenceDataHandler influenceHandler = new RevitAddInfluenceDataHandler();
    private static RevitWalkabilityDataHandler walkableHandler = new RevitWalkabilityDataHandler();

    private static Dictionary<string, IRevitModelDataHandler> revitHandlerDictionary = new Dictionary<string, IRevitModelDataHandler>() {
        ["Doors"] = doorHandler,
        ["Walls"] = wallHandler,
        ["Influence"] = influenceHandler,
        ["Walkable"] = walkableHandler
    };

    public void ApplyHandlerMethodBasedOnString(
        string handlerIdentifier, GameObject objectToModify, string valueForModification)
    {
        //Debug.Log($"DataHandlerManager attempting to modify {objectToModify.name} with value {valueForModification} using handlerID: {handlerIdentifier}.");
        IRevitModelDataHandler dataHandler;
        revitHandlerDictionary.TryGetValue(handlerIdentifier, out dataHandler);

        if (dataHandler != null)
            dataHandler.ModifyModelWithData(objectToModify, valueForModification);
        else
            Debug.LogWarning($"DataHandlerManager did not find a DataHandler associated with the term: {handlerIdentifier}.");
    }
}
