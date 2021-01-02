using System.Collections.Generic;
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
    private static RevitSpawnDataHandler spawnHandler = new RevitSpawnDataHandler();

    private static Dictionary<string, IRevitModelDataHandler> revitHandlerDictionary = new Dictionary<string, IRevitModelDataHandler>() {
        ["Doors"] = doorHandler,
        ["Walls"] = wallHandler,
        ["Influence"] = influenceHandler,
        ["Walkable"] = walkableHandler,
        ["Spawn"] = spawnHandler
    };

    public void ApplyHandlerMethodBasedOnString(
        string handlerIdentifier, 
        GameObject objectToModify, string valueForModification)
    {
        IRevitModelDataHandler dataHandler;
        revitHandlerDictionary.TryGetValue(handlerIdentifier, out dataHandler);

        if (dataHandler != null)
            dataHandler.ModifyModelWithData(objectToModify, valueForModification);
        else
            Debug.LogWarning($"DataHandlerManager did not find a DataHandler associated with the term: {handlerIdentifier}.");
    }
}
