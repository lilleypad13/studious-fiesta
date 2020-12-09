using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAGridDetermination : MonoBehaviour
{
    [SerializeField] private float unitHeight = 1.0f;
    [SerializeField] private float nodeRadius = 1.0f;
    [SerializeField] private LayerMask unwalkableMask;

    [SerializeField] private bool isUsingSphereCollisionCheck = false;

    private int obstacleProximityPenalty = 10;

    public bool DetermineWalkabilityWithRaycast(Vector3 worldPoint)
    {
        bool walkable = true;

        // Raycast
        Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.layer == 8)
                walkable = false;
            else
            {
                worldPoint.y = hit.point.y + unitHeight / 2;
            }
        }
        else
            walkable = false;

        // TODO-A: Needs to have better control of where it is performed
        // Placement is important
        if (isUsingSphereCollisionCheck && walkable == true)
            walkable = SphereCollisionCheck(worldPoint);

        //if (!walkable)
        //    movementPenalty += obstacleProximityPenalty;

        return walkable;
    }

    // TODO-A: Needs to have better control of where it is performed
    private bool SphereCollisionCheck(Vector3 worldPoint)
    {
        bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

        return walkable;
    }
}