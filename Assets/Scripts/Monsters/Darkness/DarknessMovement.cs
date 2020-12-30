using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

namespace DarknessMinion
{
    
    public class DarknessMovement 
    {
        public Vector3 direction, velocity, targetDirection;
        public bool moving; //reachedEndOfPath, wandering targetMoved;
        public NavigationTarget navTarget;

        private Seeker sekr;
        private Path navPath;
        private Rigidbody rgdBod;
        private Blocker bProvider;

        private float maxSpeed, maxAccel, switchTargetDistance;
        private AIPath pather;
        private Transform transform;
        private GraphUpdateScene graphUpdateScene;

        public DarknessMovement(Rigidbody rigidBody, Seeker seeker, AIPath path, Transform tform)
        {
            //speed = 2;
            moving = false;
            sekr = seeker;
            rgdBod = rigidBody;
            pather = path;
            sekr.pathCallback += PathComplete;
            bProvider = new Blocker();
            direction = new Vector3();
            transform = tform;
        }

        public void MoveDarkness()
        {
            if (moving && navPath != null)
            {
                direction = Vector3.Normalize(navPath.vectorPath[1] - transform.position);
                rgdBod.AddForce(direction); //* speed);
                                              //rigidbod.MovePosition(direction * speed * Time.deltaTime);
                float maxSpeedChange = maxAccel * Time.deltaTime;
                Vector2 desiredVelocity = targetDirection * maxSpeed;
                velocity = rgdBod.velocity;

                velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
                velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.y, maxSpeedChange);

                rgdBod.velocity = velocity;
            }
        }

        public void UpdatePath(Vector3 target)
        {
            if (sekr.IsDone())
                CreatePath(target);
        }

        public void RotateTowardsPlayer()
        {
            Vector3 pDir = DarknessManager.Instance.PlayerToDirection(transform.position);
			Vector3 dir = Vector3.RotateTowards(transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
            //Debug.Log("Currently the rotatation direction is at: " + dir + " or " + Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)).eulerAngles);
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        }

        public void StopMovement()
        {
            if (!sekr.IsDone())
                sekr.CancelCurrentPathRequest();
            moving = false;
            pather.canSearch = false;
            pather.canMove = false;
        }

        public void StartMovement()
        {
            pather.canMove = true;
			pather.canSearch = true;
        }

        public void ChangeSwitchDistance(float changeValue)
        {
            switchTargetDistance = changeValue;
        }

        public bool EndOrCloseToDestination()
        {
            if(pather.remainingDistance <= switchTargetDistance || pather.reachedDestination)
                return true;
            else return false;
        }

        public void UpdateDestinationPath(bool attacking)
        {
            if(attacking)
                pather.destination = navTarget.GetAttackPosition();
            else pather.destination = navTarget.GetNavPosition();
        }

        public void CreateDummyNavTarget(float elavation)
		{
			Vector3 randloc = new Vector3(UnityEngine.Random.Range(-10,10) + transform.position.x, elavation, UnityEngine.Random.Range(-5,5));
			navTarget = new NavigationTarget(randloc, elavation, NavigationTarget.NavTargetTag.Neutral);
		}

        private void PathComplete(Path p)
        {
            Debug.LogWarning("path callback complete");
            p.Claim(this);
            BlockPathNodes(p);
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
    }
}