using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 20.0f;
    public float turnSpeed = 3.0f;
    public float turnDistance = 5.0f; // PS
    public float stoppingDistance = 10.0f; // PS

    Path path; // PS
    //Vector3[] path; // Non-PS
    //int targetIndex; // Non-PS

    IEnumerator followRoutine;
    IEnumerator updatePathRoutine;

    private void Start()
    {
        followRoutine = FollowPath();
        updatePathRoutine = UpdatePath();
        StartCoroutine(updatePathRoutine);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            StopCoroutine(followRoutine);
        }
    }

    /*
     * Gives a path to the unit only if a path was successfully found
     */
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            Debug.Log("OnPathFound run");
            //StopCoroutine(followRoutine);
            path = new Path(waypoints, transform.position, turnDistance, stoppingDistance);
            Debug.Log("Number of waypoints in new path is:" + waypoints.Length);
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
            StopCoroutine(followRoutine);
            followRoutine = FollowPath();
            StartCoroutine(followRoutine);
        }
    }

    /*
     * Lets the unit request a new path if the target has moved considerably and some time has passed
     */
    IEnumerator UpdatePath()
    {
        if(Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPositionOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target.position - targetPositionOld).sqrMagnitude > squareMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPositionOld = target.position;
            }
        }
    }

    /*
     * Responsible for actually moving unit along its path
     */
    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1.0f; // Used to control unit speed throughout path
        Debug.Log("Starting Follow Path procedure.");
        while (followingPath)
        {
            Vector2 position2D = new Vector2(transform.position.x, transform.position.z);
            Debug.Log("Path index is: " + pathIndex);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(position2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(position2D) / stoppingDistance);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                // Slows unit down as it reaches the last waypoint
                speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(position2D) / stoppingDistance);

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }

    //IEnumerator FollowPath()
    //{
    //    Vector3 currentWaypoint = path[0];

    //    while (true)
    //    {
    //        if(transform.position == currentWaypoint)
    //        {
    //            targetIndex++;
    //        }
    //    }
    //}

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
