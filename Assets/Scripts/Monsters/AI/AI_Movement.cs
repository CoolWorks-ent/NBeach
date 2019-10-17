using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Darkness))]
public class AI_Movement : MonoBehaviour
{
    public IAstarAI aI;
    public Transform target;
    public float speed, maxSpeed, repathRate;
    public Path path;
    public Vector3 wayPoint, pathPoint;
    private Seeker sekr;
    public bool reachedEndOfPath, wandering;
    public Vector3[] PatrolPoints;
    private Rigidbody rigidbod;
    private Pathfinding.RVO.RVOController rVOController;

    void Awake()
    { 
        speed = 2;
        wandering = false;
    }

    void Start()
    {
        PatrolPoints = new Vector3[3];
        sekr = GetComponent<Seeker>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
        rVOController = gameObject.GetComponent<Pathfinding.RVO.RVOController>();
        aI = GetComponent<IAstarAI>();
        if(aI != null) aI.onSearchPath += UpdatePath;
    }

    public void UpdatePath()
    {
        if(aI != null)
        {
            if(wandering)
            {
                aI.destination = wayPoint;
            }
            else aI.destination = target.position;
        }
    }

    public void BeginMovement(Vector3 endPoint)
    {
        sekr.StartPath(transform.position, endPoint);
    }

    public void EndMovement()
    {
        sekr.CancelCurrentPathRequest();
    }

    void OnDisable()
    {
        if (aI != null) aI.onSearchPath -= UpdatePath;
        sekr.CancelCurrentPathRequest();
    }

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
        }
        else Debug.Log("Path return with an error " + path.error);
    }
}