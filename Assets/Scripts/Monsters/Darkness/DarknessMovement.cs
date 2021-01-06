using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

namespace DarknessMinion
{
	public class DarknessMovement : MonoBehaviour
	{
		[HideInInspector]
		public Vector3[] steeringMap;
		public Vector3 movementDirection { get; private set; } 
		//public Vector3 targetDirection{ get; private set; }
		public Vector3 position { get { return transform.position; } }

		public bool moving, attackPosition, reachEndOfPath;

		[Header("Assigned in Inspector")]
		public float rotationSpeed;
		public float switchTargetDistance;
		public float maxSpeed; 
		public float maxAccel;
		
		[Range(0, 5)]
		public float waypointLookAheadRange;

		[HideInInspector]
		public float playerDist { get; private set; }
		public float navTargetDist { get; private set; }

		[HideInInspector]
		public NavigationTarget navTarget;

		private Seeker sekr;
		private Path navPath;
		private Rigidbody rgdBod;
		//private Blocker bProvider;

		private Vector3 waypointDirection, desiredVelocity, appliedVelocity;
		private int waypointIndex;
		private float waypointDistance;
		private DarknessSteering steering;

		void Awake()
		{
			steering = new DarknessSteering();
			sekr = GetComponent<Seeker>();
			rgdBod = GetComponent<Rigidbody>();
			waypointIndex = 0;
			waypointDistance = 0;
			waypointDirection = new Vector3();
			desiredVelocity = new Vector3();
		}

		void Start()
		{
			moving = false;
			//sekr.pathCallback += PathComplete;
			//bProvider = new Blocker();
			movementDirection = new Vector3();
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
				while(true)
				{
					waypointDistance = Vector3.Distance(position, navPath.vectorPath[waypointIndex]);

					if (waypointDistance < waypointLookAheadRange)
					{
						if (waypointIndex + 1 < navPath.vectorPath.Count)
							waypointIndex++;
						else
						{
							reachEndOfPath = true;
							break;
						}
					}
					else break;
				}
				waypointDirection = Vector3.Normalize(navPath.vectorPath[waypointIndex] - transform.position);
				//this would be where I run the direction through the steering behaviors to get the seek and flee combined direction
				//then I can assign this calculated direction to the movementDirection;

				movementDirection = waypointDirection;
											  
				float maxSpeedChange = maxAccel * Time.deltaTime;
				desiredVelocity = movementDirection * maxSpeed;
				appliedVelocity = rgdBod.velocity;

				appliedVelocity.x = Mathf.MoveTowards(appliedVelocity.x, desiredVelocity.x, maxSpeedChange);
				appliedVelocity.z = Mathf.MoveTowards(appliedVelocity.z, desiredVelocity.z, maxSpeedChange);

				RotateTowardsDirection();

				rgdBod.velocity = appliedVelocity;
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
			Vector3 dir = Vector3.RotateTowards(transform.forward, movementDirection, rotationSpeed * Time.deltaTime, 0.1f);
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
				waypointIndex = 0;
				//targetDirection = navPath.vectorPath[waypointIndex];
			}
			else
			{
				p.Release(this);
				Debug.LogError("Path failed calculation for " + this + " because " + p.errorLog);
			}
		}

		/*private void BlockPathNodes(Path p)
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
		}*/

		void OnDestroy()
		{
			DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation;
		}
	}
}