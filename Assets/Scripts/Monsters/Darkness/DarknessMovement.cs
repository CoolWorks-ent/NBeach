using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

namespace DarknessMinion
{
    public class DarknessMovement : MonoBehaviour
    {
        public Vector3 direction { get; private set; } 
        public Vector3 velocity { get; private set; }
        public Vector3 targetDirection{ get; private set; }
        public Vector3 position { get { return transform.position; } }

        public Vector3[] directions;
        public float remainingDistance { get; private set; }

        public bool moving, attackPosition;
        public float rotationSpeed, switchTargetDistance, playerDist, navTargetDist;
        public NavigationTarget navTarget;

        private Seeker sekr;
        private Path navPath;
        private Rigidbody rgdBod;
        private Blocker bProvider;

        private float maxSpeed, maxAccel;
        private GraphUpdateScene graphUpdateScene;
        private DarknessSteering steering;

        void Awake()
        {
            steering = new DarknessSteering();
            sekr = GetComponent<Seeker>();
            rgdBod = GetComponent<Rigidbody>();
        }

        void Start()
        {
            moving = false;
            //sekr.pathCallback += PathComplete;
            bProvider = new Blocker();
            direction = new Vector3();
            DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
        }
        public void DistanceEvaluation(Vector3 location)
        {
            playerDist = Vector2.Distance(ConvertToVec2(transform.position), ConvertToVec2(location));
            if (navTarget != null)
            {
                navTargetDist = Vector3.Distance(transform.position, navTarget.GetNavPosition());
            }
            else navTargetDist = -1;
        }

        private Vector2 ConvertToVec2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public void MoveDarkness()
        {
            if (moving && navPath != null)
            {
                direction = Vector3.Normalize(navPath.vectorPath[1] - transform.position);
                //rgdBod.AddForce(direction); 
                                              
                float maxSpeedChange = maxAccel * Time.deltaTime;
                Vector2 desiredVelocity = targetDirection * maxSpeed;
                //velocity = rgdBod.velocity;

                velocity = new Vector3(Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange), 0,
                    Mathf.MoveTowards(velocity.z, desiredVelocity.y, maxSpeedChange));

                rgdBod.velocity = velocity;
            }
        }

        public void RotateTowardsPlayer()
        {
            Vector3 pDir = DarknessManager.Instance.PlayerToDirection(transform.position);
			Vector3 dir = Vector3.RotateTowards(transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        }

        public void RotateTowardsDirection()
        {
            Vector3 dir = Vector3.RotateTowards(transform.forward, direction, rotationSpeed * Time.deltaTime, 0.1f);
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        }

        public void StopMovement()
        {
            if(sekr != null)
            {
                if (!sekr.IsDone())
                    sekr.CancelCurrentPathRequest();
                moving = false;
            }
            //pather.canSearch = false;
            //pather.canMove = false;
        }

        public void StartMovement()
        {
            moving = true;
            //pather.canMove = true;
			//pather.canSearch = true;
        }

        public void UpdateDestinationPath(bool attacking)
        {
            if(attacking)
                 CreatePath(navTarget.GetAttackPosition());
            else CreatePath(navTarget.GetNavPosition());
        }

        public void CreateDummyNavTarget(float elavation)
		{
			Vector3 randloc = new Vector3(UnityEngine.Random.Range(-10,10) + transform.position.x, elavation, UnityEngine.Random.Range(-5,5));
			navTarget = new NavigationTarget(randloc, elavation, NavigationTarget.NavTargetTag.Neutral);
		}

        public void CreatePath(Vector3 endPoint)
        {
            //bProvider.blockedNodes.Clear();
            //Path p = ABPath.Construct(transform.position, endPoint);
            //p.traversalProvider = bProvider;
            sekr.StartPath(position, endPoint, PathComplete);
            //p.BlockUntilCalculated();
        }

        private void PathComplete(Path p)
        {
            //Debug.LogWarning("path callback complete");
            p.Claim(this);
            //BlockPathNodes(p);
            if (!p.error)
            {
                if (navPath != null)
                    navPath.Release(this);
                navPath = p;
            }
            else
            {
                p.Release(this);
                Debug.LogError("Path failed calculation for " + this + " because " + p.errorLog);
            }
        }

        private void BlockPathNodes(Path p)
        {
            foreach (GraphNode n in p.path)
            {
                bProvider.blockedNodes.Add(n);
            }
        }

        private class Blocker : ITraversalProvider
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


        void OnDestroy()
        {
            DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation;
        }
    }
}