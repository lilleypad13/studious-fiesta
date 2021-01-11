using UnityEngine;

public class RaycastAGridDetermination : MonoBehaviour
{
    [SerializeField] private float unitHeight = 1.0f;
    [SerializeField] private float checkRadius = 1.0f;
    [SerializeField] private LayerMask unwalkableMask;
    private int UNWALKABLE_LAYER_CONST = 8;
    [SerializeField] private float heightCheck = 2.0f;

    [SerializeField] private bool isUsingSphereCollisionCheck = false;
    [SerializeField] private bool isUsingUpwardRaycast = true;

    // Secondary Raycast Parameters
    private Vector3 incrementalRayMove = new Vector3(0.0f, 0.1f, 0.0f);

    public Node DownwardRaycastCheck(Node node)
    {
        Node setNode = node;
        bool walkable = true;

        // Raycast
        Ray ray = new Ray(setNode.worldPosition + Vector3.up * 50, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.layer == 8)
                walkable = false;
            //else
            //{
                setNode.worldPosition.y = hit.point.y + unitHeight / 2;
            //}
        }
        else
            walkable = false;

        // TODO-A: Needs to have better control of where it is performed
        // Placement is important
        if (isUsingSphereCollisionCheck && walkable == true)
            walkable = SphereCollisionCheck(setNode.worldPosition);

        setNode.walkable = walkable;

        return setNode;
    }

    public Node DetermineWalkabilityWithRaycast(Node node)
    {
        if (isUsingUpwardRaycast)
            return UpwardRaycastCheck(node);
        else
            return DownwardRaycastCheck(node);
    }

    private Node UpwardRaycastCheck(Node node)
    {
        Node setNode = node;
        bool walkable = true;

        // Raycast
        Ray ray = new Ray(setNode.worldPosition + Vector3.down * 50, Vector3.up);
        Ray secondRay;
        RaycastHit hit;
        RaycastHit secondHit;

        float distanceMeasure = 0.0f;

        if (Physics.Raycast(ray, out hit, 200))
        {
            if (hit.collider.gameObject.layer == UNWALKABLE_LAYER_CONST)
            {
                setNode.worldPosition.y = hit.point.y;
                walkable = false;
            }
            else
            {
                setNode.worldPosition.y = hit.point.y;

                secondRay = new Ray(hit.point + incrementalRayMove, Vector3.up);
                if (Physics.Raycast(secondRay, out secondHit, 200.0f, 1 << LayerMask.NameToLayer("Unwalkable"))) // Ray travels until hitting the unwalkableMask layer
                {
                    if (secondHit.collider.gameObject.layer == UNWALKABLE_LAYER_CONST)
                    {
                        distanceMeasure = secondHit.point.y - hit.point.y;
                        if (distanceMeasure < heightCheck)
                            walkable = false;
                    }
                }

                if (isUsingSphereCollisionCheck && walkable == true)
                    walkable = SphereCollisionCheck(setNode.worldPosition);
            }
        }
        else
            walkable = false;

        

        setNode.walkable = walkable;

        return setNode;
    }

    public float DetermineElevationWithRaycast(Vector3 worldPoint)
    {
        float elevation = 0.0f;

        // Raycast
        Ray ray = new Ray(worldPoint + Vector3.down * 50, Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200.0f))
        {
            elevation = hit.point.y + unitHeight / 2;
        }

        return elevation;
    }

    // TODO-A: Needs to have better control of where it is performed
    private bool SphereCollisionCheck(Vector3 worldPoint)
    {
        bool walkable = !(Physics.CheckSphere(worldPoint, checkRadius, unwalkableMask));

        return walkable;
    }
}