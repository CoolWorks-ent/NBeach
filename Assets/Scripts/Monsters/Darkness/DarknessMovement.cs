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

		[HideInInspector]
		public float playerDist { get; private set; }
		public float navTargetDist { get; private set; }

		[HideInInspector]
		public NavigationTarget navTarget;
		[HideInInspector]
		public Transform player, playerRoot;

		private AIPath pather;

		private DirectionNode[] directionNodes;
		private int bestDirectionIndex;

		[SerializeField,Range(0, 10)]
		private int lookAheadDistance;

		[SerializeField, Range(0, 10)]
		private float movementPrecisionDistance;

		private Vector3 pointToHighlyAvoid;

		[SerializeField]
		private LayerMask avoidLayerMask;

		void Awake()
		{
			pather = GetComponent<AIPath>();

			directionNodes = new DirectionNode[10];
			directionNodes[0] = new DirectionNode(90 * Mathf.Deg2Rad);
			directionNodes[1] = new DirectionNode(120 * Mathf.Deg2Rad);
			directionNodes[2] = new DirectionNode(150 * Mathf.Deg2Rad);
			directionNodes[3] = new DirectionNode(180 * Mathf.Deg2Rad);
			directionNodes[4] = new DirectionNode(60 * Mathf.Deg2Rad);
			directionNodes[5] = new DirectionNode(30 * Mathf.Deg2Rad);
			directionNodes[6] = new DirectionNode(0 * Mathf.Deg2Rad);
			directionNodes[7] = new DirectionNode(225 * Mathf.Deg2Rad);
			directionNodes[8] = new DirectionNode(315 * Mathf.Deg2Rad);
			directionNodes[9] = new DirectionNode(270 * Mathf.Deg2Rad);
			bestDirectionIndex = 0;
			pointToHighlyAvoid = new Vector3();
		}

		void Start()
		{
			DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
			if (player)
				playerRoot = player.root; //TODO do something with this

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

		public bool IsFacingPlayer(float minimumValue)
        {
			float closeness = Vector3.Dot(PlayerDirection(), transform.forward);
			if (closeness >= minimumValue)
				return true;
			else return false;
        }

		public Vector3 PlayerDirection()
        {
			return (player.position - transform.position).normalized;
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

		public void UpdateHighAvoidancePoint(Vector3 point)
        {
			pointToHighlyAvoid = point;
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
			foreach (DirectionNode dNode in directionNodes)
			{
				SeekLayer(dNode);
				AvoidLayer(dNode);
			}

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

			pather.destination = directionNodes[bestDirectionIndex].directionAtAngle * calculationDistance(playerDist) + this.transform.position;
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

		private void SeekLayer(DirectionNode dNode)
        {
			float dotValue = Vector3.Dot(dNode.directionAtAngle, PlayerDirection());
			if (dotValue >= 0.9f)
				dNode.seekWeight = 1.2f;
			else if (dotValue >= 0.8f && dotValue < 0.9f)
				dNode.seekWeight = 0.9f;
			else if (dotValue < 0.8f && dotValue >= 0.7f)
				dNode.seekWeight = 0.7f;
			else if (dotValue < 0.7f && dotValue >= 0.4f)
				dNode.seekWeight = 0.5f;
			else dNode.seekWeight = 0.1f;
		}

		private void AvoidLayer(DirectionNode dNode)
        {
			dNode.avoidWeight = 0;
			RaycastHit rayHit;
			float dotValue;

			if (pointToHighlyAvoid != Vector3.zero)
			{
				float avoidancePointDistance = Vector3.Distance(transform.position, pointToHighlyAvoid);
				if (avoidancePointDistance <= calculationDistance(avoidancePointDistance))
				{
					float localAvoidanceDotValue = LocalAvoidanceDotValue(dNode.directionAtAngle);
					if (localAvoidanceDotValue >= 0.8f)
						dNode.avoidWeight += -0.5f;
				}
			}

			if (Physics.SphereCast(transform.position + dNode.directionAtAngle * 1.5f, 2, dNode.directionAtAngle * calculationDistance(playerDist) * 1.5f, out rayHit, calculationDistance(playerDist) + 2, avoidLayerMask, QueryTriggerInteraction.Collide))
			{
				dotValue = Vector3.Dot(dNode.directionAtAngle, rayHit.transform.position);
				if (dotValue >= 0.6f)
					dNode.avoidWeight += -1;
				else if (dotValue <= 0.6f && dotValue > 0)
					dNode.avoidWeight += -0.5f;
			}
		}

		private float calculationDistance(float distance)
        {
			if (distance <= movementPrecisionDistance)
				return lookAheadDistance / 2;
			else return lookAheadDistance;
        }

		private float LocalAvoidanceDotValue(Vector3 direction)
        {
			return Vector3.Dot(direction, pointToHighlyAvoid.normalized);
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
			Color col = Color.red;

			foreach(DirectionNode dir in directionNodes)
            {
				if (dir.combinedWeight > 0.9f)
					col = Color.green;
				else if (dir.combinedWeight > 0.7f && dir.combinedWeight < 0.9f)
					col = Color.cyan;
				else if (dir.combinedWeight < 0.7f && dir.combinedWeight > 0.5f)
					col = Color.yellow;
				else if (dir.combinedWeight < 0.5f && dir.combinedWeight > 0)
					col = Color.magenta;
				else col = Color.white;
				Debug.DrawLine(this.transform.position, dir.directionAtAngle + this.transform.position, col);
            }

			//Gizmos.DrawSphere(directionNodes[bestDirectionIndex].directionAtAngle * 5 + this.transform.position, 2);
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