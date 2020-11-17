using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareModelForNavigation : Initializer
{
    [SerializeField] GameObject modelToPrepare;
    private ChildObjectGatherer childGatherer = new ChildObjectGatherer();
    public ChildObjectGatherer ChildGatherer { get => childGatherer; }
    private ColliderCreator colliderCreator = new ColliderCreator();
    private GlobalModelData globalModelData;

    Bounds entireModelBounds;

    private void OnDrawGizmos()
    {
        ModelBoundsFinder.VisualizeBoundsBox(entireModelBounds);
    }
    public override void Initialization()
    {
        globalModelData = GlobalModelData.Instance;

        childGatherer.CreateListOfChildrenObjects(modelToPrepare);
        colliderCreator.CreateColliders(childGatherer.AllObjectChildren);

        entireModelBounds = ModelBoundsFinder.EncapsulateGroupOfModelRenderers(childGatherer.AllObjectChildren);
        Debug.Log(ModelBoundsFinder.ReportModelBounds(entireModelBounds));
        ModelBoundsFinder.ReportListOfBounds(childGatherer.AllObjectChildren);

        globalModelData.ModelBounds = entireModelBounds;
    }
}
