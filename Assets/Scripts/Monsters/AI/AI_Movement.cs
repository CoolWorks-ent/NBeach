using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Darkness))]
public class AI_Movement : MonoBehaviour
{
    public Transform target;
    public float speed, maxSpeed, repathRate;
    public Vector3 wayPoint, pathPoint;
    private Seeker sekr;
    private Path navPath;
    public bool reachedEndOfPath, wandering, pathCalculating, targetMoved;
    public Vector3[] PatrolPoints;
    private Rigidbody rigidbod;
    private Blocker bProvider;

    void Awake()
    { 
        speed = 2;
        wandering = targetMoved = reachedEndOfPath = false;
        PathUpdate += PathTargetUpdated;
    }

    void Start()
    {
        PatrolPoints = new Vector3[3];
        sekr = GetComponent<Seeker>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
        sekr.pathCallback += PathComplete;
        bProvider = new Blocker();
    }

    public void UpdatePath(Vector3 target)
    {
        if(!pathCalculating)
            CreatePath(target);
    }

    private void PathComplete(Path p)
    {
        Debug.LogWarning("path callback complete");
        pathCalculating = false;
        p.Claim(this);
        BlockPathNodes(p);
        if(!p.error)
        {
            if(navPath != null) 
                navPath.Release(this);
        }
        else 
        {
            p.Release(this);
            Debug.LogError("Path failed calculation for " + this + " because " + p.errorLog);
        }
    }

    private void PathTargetUpdated(bool b)
    {
        targetMoved = b;
    }

    private void BlockPathNodes(Path p)
    {
        foreach(GraphNode n in p.path)
        {
            bProvider.blockedNodes.Add(n);
        }
        //Debug.Break();
    }

    public void CreatePath(Vector3 endPoint)
    {
        bProvider.blockedNodes.Clear();
        pathCalculating = true;
        Path p = ABPath.Construct(transform.position, endPoint);
        p.traversalProvider = bProvider;
        sekr.StartPath(p, null);
        p.BlockUntilCalculated();
    }

    public void EndMovement()
    {
        if(pathCalculating)
            sekr.CancelCurrentPathRequest();
    }

    void OnDisable()
    {
        EndMovement();
    }

    class Blocker : ITraversalProvider
    {
        public HashSet<GraphNode> blockedNodes = new HashSet<GraphNode>();
        public bool CanTraverse(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.CanTraverse(path, node) && !blockedNodes.Contains(node);
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }

    public delegate void AI_MovementEvent<T>(T obj);

    public static event AI_MovementEvent<bool> PathUpdate;

    public static void OnPathUpdate(bool b)
    {
        if(PathUpdate != null)
            PathUpdate(b);
    }
}