using System;
using System.Collections.Generic;
using UnityEngine;


public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    Pathfinding pathfinder;

    public static PathRequestManager instance;

    bool isProcessingPath;
    public Transform player;

    private void Awake()
    {
        instance = this;
        pathfinder = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest freshRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(freshRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }


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

        public PathRequest(Vector3 _pStart, Vector3 _pEnd, Action<Vector3[], bool> _hollaback)
        {
            pathStart = _pStart;
            pathEnd = _pEnd;
            callback = _hollaback;
        }
    }
}