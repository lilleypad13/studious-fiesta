using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affinity
{
    public readonly string Name;

    public int AffinityValue
    {
        get => affinityValue;
        set
        {
            if (value > MathArchCost.Instance.MAX_AFFINITY)
                affinityValue = MathArchCost.Instance.MAX_AFFINITY;
            else
                affinityValue = value;
        }
    }
    private int affinityValue;

    public Affinity(string _name)
    {
        Name = _name;
    }

    public Affinity(string _name, int _affinityValue)
    {
        Name = _name;
        AffinityValue = _affinityValue;
    }
}

public class UnitSimple : MonoBehaviour
{
    public Vector3 target;
    public float speed = 20.0f;
    private Vector3[] waypoints;
    private int targetIndex;

    IEnumerator followRoutine;

    public Dictionary<string, Affinity> affinityTypes = new Dictionary<string, Affinity>();

    private void Awake()
    {
        if (affinityTypes.Count == 0)
            PopulateDictionaryFromGlobal();
    }

    // Agents determine their path here in Start
    private void Start()
    {
        DebugReportAffinityValues();
        followRoutine = FollowPath();
    }

    /*
     * Allows spawning tools to ensure this action is called after a newly instantiated agent has its parameters 
     * properly set.
     */
    public void AgentRequestPath()
    {
        Debug.Log(gameObject.name + " has requested a path.");
        PathRequestManagerSimple.RequestPath(transform.position, target, OnPathFound, this);
    }

    public void OnPathFound(Vector3[] newWaypointPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            waypoints = newWaypointPath;
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


    // Most likely needs run when creating a new agent.
    // Can be possibly covered by running in this Awake method.
    public void PopulateDictionaryFromGlobal()
    {
        foreach (KeyValuePair<string, ArchitecturalElementContainer> container in GlobalModelData.architecturalElementContainers)
        {
            if (!affinityTypes.ContainsKey(container.Key))
            {
                Affinity affinity = new Affinity(container.Key);
                affinityTypes.Add(container.Key, affinity);
            }
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

    #region Debugging

    private void DebugReportAffinityValues()
    {
        string debugMessage = $"{name} Affinity Values:\n";

        foreach (KeyValuePair<string, Affinity> aff in affinityTypes)
        {
            debugMessage += $"Type: {aff.Key}; Value: {aff.Value.AffinityValue}\n";
        }

        Debug.Log(debugMessage);
    }

    #endregion
}
