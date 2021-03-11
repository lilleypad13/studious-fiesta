using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareModelForNavigation : Initializer
{
    private ChildObjectGatherer childGatherer = new ChildObjectGatherer();
    private ColliderCreator colliderCreator = new ColliderCreator();

    [SerializeField] private GameObject modelToPrepare;

    private List<GameObject> objectsInEntireModel = new List<GameObject>();
    private Bounds entireModelBounds;

    //private void OnDrawGizmos()
    //{
    //    ModelBoundsFinder.VisualizeBoundsBox(entireModelBounds);
    //}

    public override void Initialization()
    {
        // Gathers all children objects within the entire model and creates a list of them
        objectsInEntireModel = childGatherer.CreateListOfChildrenObjects(modelToPrepare);

        // Applies colliders to entire list of objects within the overall model
        // Adds tags to make objects selectable
        // Add component for selectable logic
        colliderCreator.CreateColliders(objectsInEntireModel);
        AddSelectableComponent(objectsInEntireModel);

        // Creation of model bounds
        entireModelBounds = ModelBoundsFinder.EncapsulateGroupOfModelRenderers(objectsInEntireModel);

        // Set global values for use by other classes
        GlobalModelData.Instance.ObjectsInEntireModel = objectsInEntireModel;
        GlobalModelData.Instance.ModelBounds = entireModelBounds;
    }


    private void AddSelectableComponent(List<GameObject> objects)
    {
        foreach (GameObject item in objects)
        {
            item.AddComponent<Selectable>();
        }
    }
}
