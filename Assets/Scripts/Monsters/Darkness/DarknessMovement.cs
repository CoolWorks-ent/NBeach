using UnityEngine;
using Pathfinding;

namespace DarknessMinion
{
	public class DarknessMovement : MonoBehaviour
	{
		public bool reachedEndofPath { get { return pather.reachedDestination; } }

		[HideInInspector]
		public float playerDist { get; private set; }
		[HideInInspector]
		public Darkness darkness;

		[HideInInspector]
		public Transform player;

		private int bestDirectionIndex;

		private Vector3 destinationPoint;
		//public AttackZone darkAttackZone; //TODO Replace this with just a reference to a Vector3

		//private NavigationTarget attackZoneNavTarget;

		private AIPath pather;
		private DirectionNode[] directionNodes;
		
		[SerializeField,Range(0, 10)]
		private float pathSetDistance;

		//TODO Create another variable for editing the distance at which the path ahead is checked. Or maybe look into the values currently available and see if tweaks can make the behavior better
		//At some point I need to make the path shorter if the set path goes past the player 

		[SerializeField, Range(0, 10)]
		private float higherPrecisionAvoidanceThreshold;

		[SerializeField]
		private LayerMask avoidLayerMask;


		void Awake()
		{
			pather = GetComponent<AIPath>();

			directionNodes = new DirectionNode[16];
			destinationPoint = Vector3.zero;

			float angle, dAngle = 0;
			for(int i = 0; i < directionNodes.Length; i++)
			{
				dAngle = ((360.0f / directionNodes.Length) * (float)i);
				angle = Mathf.Deg2Rad * dAngle;
				DirectionNode n = new DirectionNode(angle, dAngle);
				directionNodes[i] = n;
				//n.CreateDebugText(new Vector3(n.directionAtAngle.x, transform.position.y, n.directionAtAngle.z), this.transform);
			}
			bestDirectionIndex = 0;
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
			return closeness >= minimumValue;
		}

		public Vector3 PlayerDirection(bool moving)
		{
			if (!moving && destinationPoint != Vector3.zero)
			{
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

			//attackZoneNavTarget = 
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
			Vector3 direction = PlayerDirection(true);
			float dotValue = Vector3.Dot(dNode.directionAtAngle, direction);
			dNode.SetDirLength(direction.magnitude);

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
			float dotValue, distanceNorm;

			if (Physics.SphereCast(transform.position + dNode.directionAtAngle, 2, dNode.directionAtAngle * CalculationDistance(playerDist) * 1.5f, out rayHit, CalculationDistance(playerDist) + 2, avoidLayerMask, QueryTriggerInteraction.Collide))
			{
				dotValue = Vector3.Dot(dNode.directionAtAngle, rayHit.transform.position.normalized);
				distanceNorm = Vector3.Distance(transform.position, rayHit.transform.position) / 10.5f;
				if (dotValue >= 0.6f)
					dNode.avoidWeight += -1;
				else if (dotValue <= 0.6f && dotValue > 0)
					dNode.avoidWeight += -0.5f;

				if (distanceNorm > 0.2f)
					dNode.avoidWeight -= distanceNorm;
			}
		}

		private float CalculationDistance(float distance)
		{
			if (distance < higherPrecisionAvoidanceThreshold)
				return pathSetDistance / 2;
			else return pathSetDistance;
		}


	#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			Color col = Color.red;
			Vector3 lineStart, lineEnd;

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
				lineStart = this.transform.position + dir.directionAtAngle;
				lineEnd = dir.directionAtAngle * (dir.combinedWeight * CalculationDistance(playerDist)) + this.transform.position ;
				Debug.DrawLine(lineStart, lineEnd, col);
			}
		}
	#endif

		private class DirectionNode
		{
			public float angle { get; private set; }
			public float degAngle { get; private set; }
			public float avoidWeight, seekWeight;
			public float combinedWeight { get { return avoidWeight + seekWeight; } }

			public float dirLength { get; private set; }

			public Vector3 directionAtAngle { get; private set; }

			public TextMesh debugText {get; private set;}

			public DirectionNode(float rAngle, float dAngle)
			{
				angle = rAngle;
				degAngle = dAngle;
				avoidWeight = 0;
				seekWeight = 0;
				directionAtAngle = Vector3.zero;
				dirLength = 0;
			}

			public void SetDirection(Vector3 v)
			{ 
				directionAtAngle = v; 
			}

			public void SetDirLength(float v)
			{
				dirLength = v;
			}

			public void SetDebugText(string text)
			{
				debugText.text = text;
			}
		}
	}
}