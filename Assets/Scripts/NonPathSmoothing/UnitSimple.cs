using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSimple : MonoBehaviour
{
    public Transform target;
    public float speed = 20.0f;
    private Vector3[] waypoints;
    private int targetIndex;

    IEnumerator followRoutine;

    // Architectural Parameters
    // Measures their affinity towards a certain architectural element (higher = moves towards it)
    public int Window
    {
        get { return window; }
        set
        {
            if (value > MathArchCost.Instance.MAX_AFFINITY)
                window = MathArchCost.Instance.MAX_AFFINITY;
            else
                window = value;
        }
    }
    [SerializeField]private int window = 0;

    public int Connectivity
    {
        get { return connectivity; }
        set
        {
            if (value > MathArchCost.Instance.MAX_AFFINITY)
                connectivity = MathArchCost.Instance.MAX_AFFINITY;
            else
                connectivity = value;
        }
    }
    [SerializeField] private int connectivity = 0;

    // Agents determine their path here in Start
    private void Start()
    {
        followRoutine = FollowPath();
        //PathRequestManagerSimple.RequestPath(transform.position, target.position, OnPathFound, this);
    }

    /*
     * Allows spawning tools to ensure this action is called after a newly instantiated agent has its parameters 
     * properly set.
     */
    public void AgentRequestPath()
    {
        Debug.Log(gameObject.name + " has requested a path.");
        PathRequestManagerSimple.RequestPath(transform.position, target.position, OnPathFound, this);
    }

    public void OnPathFound(Vector3[] newWaypointPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            waypoints = newWaypointPath;
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");
            StopCoroutine(followRoutine);
            StartCoroutine(followRoutine);
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = waypoints[0];

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= waypoints.Length)
                {
                    yield break;
                }
                currentWaypoint = waypoints[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(waypoints != null)
        {
            for(int i = targetIndex; i < waypoints.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(waypoints[i], Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, waypoints[i]);
                }
                else
                {
                    Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
                }
            }
        }
    }
}
