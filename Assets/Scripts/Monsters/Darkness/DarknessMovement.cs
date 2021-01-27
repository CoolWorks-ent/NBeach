using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace DarknessMinion
{
	public class DarknessMovement : MonoBehaviour
	{
		[HideInInspector]
		public Vector3[] steeringMap;
		public bool reachedEndofPath { get { return pather.reachedDestination; } }
		public bool closeToPlayer { get; private set; }
		
		[Range(0, 5)]
		public float playerCloseRange;

		[Range(0, 10)]
		public int lookAheadDistance;

		[HideInInspector]
		public float playerDist { get; private set; }
		public float navTargetDist { get; private set; }

		[HideInInspector]
		public NavigationTarget navTarget;
		[HideInInspector]
		public Transform player;

		private Seeker sekr;
		private AIPath pather;

		private DirectionNode[] directionNodes;
		private int bestDirectionIndex;

		private LayerMask avoidLayerMask;

		void Awake()
		{
			sekr = GetComponent<Seeker>();
			pather = GetComponent<AIPath>();

			directionNodes = new DirectionNode[8];
			directionNodes[0] = new DirectionNode(90 * Mathf.Deg2Rad);
			directionNodes[1] = new DirectionNode(120 * Mathf.Deg2Rad);
			directionNodes[2] = new DirectionNode(140 * Mathf.Deg2Rad);
			directionNodes[3] = new DirectionNode(160 * Mathf.Deg2Rad);
			directionNodes[4] = new DirectionNode(70 * Mathf.Deg2Rad);
			directionNodes[5] = new DirectionNode(50 * Mathf.Deg2Rad);
			directionNodes[6] = new DirectionNode(30 * Mathf.Deg2Rad);
			directionNodes[7] = new DirectionNode(270* Mathf.Deg2Rad);
			bestDirectionIndex = 0;
		}

		void Start()
		{
			//sekr.pathCallback += PathComplete;
			//bProvider = new Blocker();
			DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
			avoidLayerMask = LayerMask.GetMask("Darkness", "Environment");
		}

		public void DistanceEvaluation()
		{
			playerDist = Vector2.Distance(ConvertToVec2(transform.position), ConvertToVec2(DarknessManager.Instance.playerVector));
			if (navTarget != null)
			{
				navTargetDist = Vector3.Distance(transform.position, navTarget.navPosition);
			}
			else navTargetDist = -1;
		}

		private Vector2 ConvertToVec2(Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		public void RotateTowardsPlayer()
		{
			Vector3 pDir = DarknessManager.Instance.DirectionToPlayer(transform.position);
			Vector3 dir = Vector3.RotateTowards(transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
		}

		public void StopMovement()
		{
			pather.canSearch = false;
			pather.canMove = false;
		}

		public void StartMovement()
		{
			pather.canMove = true;
			pather.canSearch = true;
		}

		/*Add functions for applying steering behaviors
		* Choose several points in a circle that would be the starting candidates for movement
		* Vectors closer towards the direction of the player get some positive weight
		* Vectors close to the directions of other Darkness get some negative weight
		* Somehow we'll have a direction that is weighted most favorable and we move in that direction
		* This will not run very often. Still need to figure out how I want that to happen. Maybe decided in state cooldowns
		*/

		public void UpdatePathDestination()
        {
			GenerateVectorPaths();
			SeekLayer();
			AvoidLayer();

			//Once the layers are calulated with weights narrow down the path that leads closer to the player
			//Once the direction is chosen set the navtarget to a point along the direction vector
			bestDirectionIndex = 0;
			for (int i = 0; i < directionNodes.Length; i++)
            {
				if(i+1 <= directionNodes.Length-1)
                {
					if(directionNodes[bestDirectionIndex].combinedWeight < directionNodes[i].combinedWeight)
						bestDirectionIndex = i;
				}
            }

			pather.destination = directionNodes[bestDirectionIndex].directionAtAngle * (lookAheadDistance + 2) + this.transform.position;
        }


		private void GenerateVectorPaths()
        {
			//Generate vectors in several directions in a circle around the Darkness
			//I want points at certain angles all around the Darkness and I want to save those to an array
			for (int i = 0; i < directionNodes.Length; i++)
			{
				directionNodes[i].SetDirection(new Vector3(Mathf.Cos(directionNodes[i].angle), 0.1f, Mathf.Sin(directionNodes[i].angle)));
			}
		}

		private void SeekLayer()
        {
			//Apply a positive weight to each direction in the seekMap
			//The weights should be added positively to directions that are most aligned to the player direction
			//For example: if the player is (0.1, 0, 0.9) and the closest direction is (0, 0, 0.5) this direction gets the highest weight
			//Use the dot product to figure out how aligned the seek vector is to the player direction
			//Set falloff rates for each of the vectors
			//Vectors with dot values of 1 - 0.8 are +1 weight
			//Vectors with dot values of 0.7 - 0.4 are +0.5 weight
			//Vectors with dot values below 0.4 are +0.1 weight

			Vector3 playerDirection = (player.position - transform.position).normalized;
			float dotValue = 0;

			foreach(DirectionNode dNode in directionNodes)
            {
				dotValue = Vector3.Dot(dNode.directionAtAngle, playerDirection) * 2;
				if (dotValue >= 0.8f)
					dNode.seekWeight = 1f;
				else if (dotValue < 0.8f && dotValue >= 0.4f)
					dNode.seekWeight = 0.5f;
				else dNode.seekWeight = 0.1f;
			}
		}

		private void AvoidLayer()
        {
			//Apply a negative weight to each direction in the avoidMap
			//Should do some kind of detection in this section to figure out if a direction is headed toward a Darkness or obstacle
			//I can put a trigger on the Darkness and apply negative values to anything not the player in the avoidanceMap
			//Get the directions of objects to avoid
			//Apply the dot product to direction I want to go towards and the obstacle direction
			//Set falloff for each of the vectors
			//Vectors with dot values of 1 - 0.6 are -1 weight	
			//Vectors with dot values of 0.6 - 0 are -0.5 weight
			//Vectors with dot values of 0 or below are not given a weight

			RaycastHit rayHit;
			float dotValue = 0;

			foreach(DirectionNode dNode in directionNodes)
            {
				if (Physics.SphereCast(transform.position + dNode.directionAtAngle * 1.5f, 2, dNode.directionAtAngle * lookAheadDistance * 1.5f, out rayHit, lookAheadDistance+2, avoidLayerMask, QueryTriggerInteraction.Collide ))
				{
					//dNode.avoidWeight 
					dotValue = Vector3.Dot(dNode.directionAtAngle, rayHit.transform.position);
					if (dotValue >= 0.6f)
						dNode.avoidWeight = -1;
					else if (dotValue <= 0.6f && dotValue >= 0)
						dNode.avoidWeight = -0.5f;
					else dNode.avoidWeight = 0;
				}
				else dNode.avoidWeight = 0;
            }
        }

		/*private void PathComplete(Path p)
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
		}*/

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

		void OnDrawGizmos()
        {
			foreach(DirectionNode dir in directionNodes)
            {
				Debug.DrawLine(this.transform.position, dir.directionAtAngle + this.transform.position, Color.green);
            }

			Gizmos.DrawSphere(directionNodes[bestDirectionIndex].directionAtAngle * 5 + this.transform.position, 2);
        }

		void OnDestroy()
		{
			DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation;
		}

		private class DirectionNode
        {
			public float angle { get; private set; }
			public float avoidWeight, seekWeight, baseWeight;
			public float combinedWeight { get { return avoidWeight + seekWeight; } }

			public Vector3 directionAtAngle { get; private set; }

			public DirectionNode(float dAngle)
            {
				angle = dAngle;
				avoidWeight = 0;
				seekWeight = 0;
				directionAtAngle = Vector3.zero;
            }

			public void SetDirection(Vector3 v)
			{ 
				directionAtAngle = v; 
			}
        }
	}
}