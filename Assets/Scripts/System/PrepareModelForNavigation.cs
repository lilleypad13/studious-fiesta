using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareModelForNavigation : Initializer
{
    [SerializeField] GameObject modelToPrepare;
    private ChildObjectGatherer childGatherer = new ChildObjectGatherer();
    public ChildObjectGatherer ChildGatherer
    {
        get => childGatherer;
    }

    private ColliderCreator colliderCreator = new ColliderCreator();

    public override void Initialization()
    {
        childGatherer.CreateListOfChildrenObjects(modelToPrepare);
        colliderCreator.CreateColliders(childGatherer.AllObjectChildren);
    }
}
