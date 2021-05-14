using UnityEngine;
using Pathfinding;

namespace DarknessMinion
{
	public class DarknessMovement : MonoBehaviour
	{
		public bool reachedEndofPath { get { return pather.reachedDestination; } }
		public bool closeToPlayer { get; private set; }
		public bool inAttackZone { get; private set; }

		[HideInInspector]
		public float playerDist { get; private set; }

		[HideInInspector]
		public Transform player;

		public DarknessAttackZone darkAttackZone;

		private NavigationTarget attackZoneNavTarget;

		private AIPath pather;

		private DirectionNode[] directionNodes;
		private int bestDirectionIndex;

		[SerializeField,Range(0, 10)]
		private int lookAheadDistance;

		[SerializeField, Range(0, 10)]
		private float movementPrecisionDistance;

		//private Vector3 pointToHighlyAvoid;

		[SerializeField]
		private LayerMask avoidLayerMask;


		void Awake()
		{
			pather = GetComponent<AIPath>();

			directionNodes = new DirectionNode[16];

			for(int i = 1; i < directionNodes.Length+1; i++)
			{
				directionNodes[i-1] = new DirectionNode(Mathf.Deg2Rad * ((360 / directionNodes.Length) * i));
			}
			bestDirectionIndex = 0;
			//pointToHighlyAvoid = new Vector3();
		}

		void OnEnable() { DarkEventManager.UpdateDarknessDistance += DistanceEvaluation; }
		void OnDisable() { DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation; }


		public void DistanceEvaluation()
		{
			playerDist = Vector2.Distance(ConvertToVec2(transform.position), ConvertToVec2(DarknessManager.Instance.playerVector));
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
			float closeness = Vector3.Dot(PlayerDirection(false), transform.forward);
			if (closeness >= minimumValue)
				return true;
			else return false;
		}

		public Vector3 PlayerDirection(bool moving)
		{
			if (moving)
			{
				//TODO: Fix this, check if attackZoneNavTarget is valid
				if (darkAttackZone.InTheZone(ConvertToVec2(transform.position)))
					return (player.position - transform.position).normalized;
				else return (attackZoneNavTarget.navPosition - transform.position).normalized;
			}
			else return (player.position - transform.position).normalized; 
		}

		public void StopMovement()
		{
			pather.canSearch = false;
			pather.canMove = false;
		}

		public void StartMovement()
		{
			darkAttackZone = DarknessAttackZone.Instance;

			attackZoneNavTarget = darkAttackZone.RequestPointInsideZone(transform.position.y);
			pather.canMove = true;
			pather.canSearch = true;
		}

		/*public void UpdateHighAvoidancePoint(Vector3 point)
		{
			pointToHighlyAvoid = point;
		}*/

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

			pather.destination = directionNodes[bestDirectionIndex].directionAtAngle * CalculationDistance(playerDist) + this.transform.position;
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
			float dotValue = Vector3.Dot(dNode.directionAtAngle, PlayerDirection(true));

			if (dotValue > 0)
				dNode.seekWeight = dotValue;
			else if (dotValue == 0)
				dNode.seekWeight = 0.1f;
			else dNode.seekWeight = 0.05f;
		}

		private void AvoidLayer(DirectionNode dNode)
		{
			dNode.avoidWeight = 0;
			RaycastHit rayHit;
			float dotValue;

			/*if (pointToHighlyAvoid != Vector3.zero)
			{
				float avoidancePointDistance = Vector3.Distance(transform.position, pointToHighlyAvoid);
				if (avoidancePointDistance <= lookAheadDistance) //CalculationDistance(avoidancePointDistance))
				{
					float localAvoidanceDotValue = LocalAvoidanceDotValue(dNode.directionAtAngle);
					if (localAvoidanceDotValue >= 0.7f)
						dNode.avoidWeight += -0.8f;
				}
			}*/

			if (Physics.SphereCast(transform.position + dNode.directionAtAngle * 1.5f, 2, dNode.directionAtAngle * CalculationDistance(playerDist) * 1.5f, out rayHit, CalculationDistance(playerDist) + 2, avoidLayerMask, QueryTriggerInteraction.Collide))
			{
				dotValue = Vector3.Dot(dNode.directionAtAngle, rayHit.transform.position.normalized);
				if (dotValue >= 0.6f)
					dNode.avoidWeight += -1;
				else if (dotValue <= 0.6f && dotValue > 0)
					dNode.avoidWeight += -0.5f;
			}
		}

		private float CalculationDistance(float distance)
		{
			if (distance <= movementPrecisionDistance)
				return lookAheadDistance / 2;
			else return lookAheadDistance;
		}

		/*private float LocalAvoidanceDotValue(Vector3 direction)
		{
			return Vector3.Dot(direction, pointToHighlyAvoid.normalized);
		}*/

	#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			Color col = Color.red;
			//Gizmos.DrawCube(attackZoneNavTarget, Vector3.one*0.5f);

			foreach (DirectionNode dir in directionNodes)
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
				Debug.DrawLine(this.transform.position + dir.directionAtAngle * 2.9f, dir.directionAtAngle + this.transform.position, col);
			}
		}
	#endif

		private bool WithinAttackZone()
        {
			return darkAttackZone.InTheZone(ConvertToVec2(transform.position));
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