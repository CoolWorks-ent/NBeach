using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class Darkness_Movement : MonoBehaviour
{
    public Transform target;
    public Vector3 wayPoint, pathPoint, direction;
    public bool moving;
    public bool reachedEndOfPath, wandering, targetMoved;
    
    private Seeker sekr;
    private Path navPath;
    private Rigidbody rigidbod;
    private Blocker bProvider;

    void Awake()
    { 
        //speed = 2;
        moving = false;
        wandering = targetMoved = reachedEndOfPath = false;
        sekr = GetComponent<Seeker>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
    }

    void Start()
    {
        
        sekr.pathCallback += PathComplete;
        bProvider = new Blocker();
        direction = new Vector3();
    }

    void FixedUpdate()
    {
        if(moving && navPath != null)
        {
            direction = Vector3.Normalize(navPath.vectorPath[1] - this.transform.position);
            rigidbod.AddForce(direction); //* speed);
            //rigidbod.MovePosition(direction * speed * Time.deltaTime);
        }
    }

    public void UpdatePath(Vector3 target)
    {
        if(sekr.IsDone())
            CreatePath(target);
    }

    private void PathComplete(Path p)
    {
        Debug.LogWarning("path callback complete");
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
        Path p = ABPath.Construct(transform.position, endPoint);
        p.traversalProvider = bProvider;
        sekr.StartPath(p, null);
        p.BlockUntilCalculated();
    }

    public void EndMovement()
    {
        if(!sekr.IsDone())
            sekr.CancelCurrentPathRequest();
        moving = false;
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