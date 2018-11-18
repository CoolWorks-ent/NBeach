using UnityEngine;
using System.Collections;
using Pathfinding;

public class AI_Movement : MonoBehaviour
{
    public IAstarAI aI;
    public Transform target;
    public float waypointDistance,
                    speed;

    public int currentWaypoint;
    public Path path;

    private Seeker sekr;
    public bool endOfPath;
    private Rigidbody rigidbod;
    private Pathfinding.RVO.RVOController rVOController;

    void Awake()
    {
        speed = 2;
        waypointDistance = 3;
        currentWaypoint = 0;
    }

    void Start()
    {
        sekr = GetComponent<Seeker>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
        rVOController = gameObject.GetComponent<Pathfinding.RVO.RVOController>();

        //sekr.pathCallback += OnPathComplete;
    }

    public void UpdatePath()
    {
        sekr.StartPath(transform.position, target.position, OnPathComplete);
    }

    void FixedUpdate()
    {
        if(path == null)
            return;
        else 
        {
            float distanceToWaypoint = 0;
            endOfPath = false;
            while(true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if(distanceToWaypoint < waypointDistance)
                {
                    if(currentWaypoint+1 < path.vectorPath.Count)
                        currentWaypoint++;
                    else
                    {
                        endOfPath = true;
                        break;
                    }
                }
                else break;
            }
            
            //rigidbod.MovePosition(path.vectorPath[currentWaypoint]);
            rVOController.SetTarget(path.vectorPath[currentWaypoint], speed, speed+3);
            Vector3 delta = rVOController.CalculateMovementDelta(transform.position, Time.deltaTime);
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            
            rigidbod.AddForce(dir * speed * Time.fixedDeltaTime);
        }
    }

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
        else Debug.Log("Path return with an error " + path.error);
    }
}