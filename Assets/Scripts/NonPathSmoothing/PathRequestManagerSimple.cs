using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManagerSimple : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManagerSimple instance;
    PathfindingHeapSimple pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathfindingHeapSimple>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, 
        Action<Vector3[], bool> callback, UnitSimple agent)
    {
        //Debug.Log("Step 1: RequestPath called.");
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, agent);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        //Debug.Log("Step 2: TryProcessNext called");
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            //Debug.Log("Step 3: TryProcessNext if statement satisfied");
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.agent);
        }
    }

    /*
     * Final step in processing a path for an agent before looking to start the next one
     * Uses currentPathRequest.callback to call the OnPathFound method within the proper UnitSimple class 
     * instance to give the correct agent its determined waypoint path.
     */
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public UnitSimple agent;

        public PathRequest(Vector3 _start, Vector3 _end, 
            Action<Vector3[], bool> _callback, UnitSimple _agent)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            agent = _agent;
        }
    }
}
