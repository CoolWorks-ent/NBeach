using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Darkness))]
public class AI_Movement : MonoBehaviour
{
    public Transform target;
    public float speed, maxSpeed, repathRate;
    public Vector3 wayPoint, pathPoint, direction;
    public bool moving;
    private Seeker sekr;
    private Path navPath;
    public bool reachedEndOfPath, wandering, pathCalculating, targetMoved;
    public Vector3[] PatrolPoints;
    private Rigidbody rigidbod;
    private Blocker bProvider;

    void Awake()
    { 
        speed = 2;
        moving = false;
        wandering = targetMoved = reachedEndOfPath = false;
    }

    void Start()
    {
        PatrolPoints = new Vector3[3];
        sekr = GetComponent<Seeker>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
        sekr.pathCallback += PathComplete;
        bProvider = new Blocker();
        direction = new Vector3();
    }

    void FixedUpdate()
    {
        if(moving && navPath != null)
        {
            direction = Vector3.Normalize(navPath.vectorPath[0] - this.transform.position);
            rigidbod.AddForce(direction * speed * Time.deltaTime, ForceMode.VelocityChange);
            //rigidbod.MovePosition(direction * speed * Time.deltaTime);
        }
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
            navPath = p;
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
        moving = false;
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
}