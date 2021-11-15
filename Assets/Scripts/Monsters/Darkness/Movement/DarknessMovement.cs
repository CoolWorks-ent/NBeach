using UnityEngine;
using Pathfinding;

namespace DarknessMinion.Movement
{
	[System.Serializable]
	public class DarknessMovement 
	{
		[HideInInspector]
		public float playerDist { get; private set; }

		[HideInInspector]
		public Transform player;

		private Steering steering;

		private int bestDirectionIndex;
		private DirectionNode[] directionNodes;
		
		[SerializeField,Range(0, 10)]
		private float pathSetDistance;

		[SerializeField, Range(0, 10)]
		private float higherPrecisionAvoidanceThreshold;

		[SerializeField, Range(0, 0.5f)]
		private float seekThreshold;

		[SerializeField]
		private LayerMask avoidLayerMask;

		[SerializeField, Range(5, 20)] 
		private float maxAccel;
		[SerializeField, Range(0.1f, 5)]
		private float maxSpeed;
		[SerializeField, Range(1, 360)]
		private float rotationSpeed;
		
		private Vector3 velocity;
		public Vector2 moveDirection { get; private set; }

		void Awake()
		{
			rgdBod = GetComponent<Rigidbody>();

			directionNodes = new DirectionNode[16];
			velocity = rgdBod.velocity;
			moveDirection = Vector2.zero;

			float angle, dAngle = 0;
			for(int i = 0; i < directionNodes.Length; i++)
			{
				dAngle = ((360.0f / directionNodes.Length) * (float)i);
				angle = Mathf.Deg2Rad * dAngle;
				DirectionNode n = new DirectionNode(angle, dAngle);
				directionNodes[i] = n;
			}
			bestDirectionIndex = 0;
			steering = new Steering();
		}

		void OnEnable() { DarkEventManager.UpdateDarknessDistance += DistanceEvaluation; }
		void OnDisable() { DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation; }


		public void DistanceEvaluation()
		{
			playerDist = Vector2.Distance(transform.position.ToVector2(), DarknessManager.Instance.playerVector.ToVector2());
		}

		public void RotateTowardsPlayer()
		{
			Vector3 dir = Vector3.RotateTowards(transform.forward, PlayerDirection(), 2.0f * Time.deltaTime, 0.1f);
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
		}
		
		private void RotateTowardsDirection(Vector3 mDir) 
		{
			Vector3 dir = Vector3.RotateTowards(transform.forward, mDir,  Time.deltaTime * rotationSpeed, 0.1f);
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
		}

		public bool IsFacingPlayer(float minimumValue)
		{
			float closeness = Vector3.Dot(PlayerDirection(), transform.forward);
			return closeness >= minimumValue;
		}

		public Vector3 PlayerDirection()
		{
			return (player.position - transform.position).normalized;
		}

		public void StopMovement()
		{
			moveDirection = Vector2.zero;
		}
		
		public void MoveBody()
		{
			if (moveDirection != Vector2.zero)
			{
				float maxSpeedChange = maxAccel * Time.deltaTime;
				Vector2 desiredVelocity = moveDirection * maxSpeed;

				velocity = rgdBod.velocity;

				velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
				velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.y, maxSpeedChange);

				rgdBod.velocity = new Vector3(velocity.x, rgdBod.velocity.y, velocity.z);
				RotateTowardsDirection(velocity);
			}
		}

		public void DetermineBestDirection(Vector2 destination) //TODO pass in a target based on the 
		{
			Vector2 position = transform.position.ToVector2();
			GenerateDirectionVectors();
			foreach (DirectionNode dNode in directionNodes)
			{
				steering.Seek(dNode, position, destination, higherPrecisionAvoidanceThreshold, playerDist);
				steering.Avoid(dNode, position, higherPrecisionAvoidanceThreshold, CalculationDistance(playerDist), avoidLayerMask);
			}

			bestDirectionIndex = 0;
			for (int i = 0; i < directionNodes.Length; i++)
			{
				if(i+1 <= directionNodes.Length-1)
				{
					if (directionNodes[bestDirectionIndex].combinedWeight < directionNodes[i].combinedWeight)
					{
						float directionDifference = Mathf.Abs(directionNodes[bestDirectionIndex].combinedWeight -
						                                      directionNodes[i].combinedWeight);
						if(directionDifference > seekThreshold)
							bestDirectionIndex = i;
					}
				}
			}

			moveDirection = directionNodes[bestDirectionIndex].directionAtAngle;
		}


		private void GenerateDirectionVectors()
		{
			//Generate vectors in several directions in a circle around the Darkness
			//I want points at certain angles all around the Darkness and I want to save those to an array
			for (int i = 0; i < directionNodes.Length; i++)
			{
				directionNodes[i].SetDirection(new Vector2(Mathf.Cos(directionNodes[i].angle), Mathf.Sin(directionNodes[i].angle)));
			}
		}

		private float CalculationDistance(float distance)
		{
			if (distance < higherPrecisionAvoidanceThreshold)
				return pathSetDistance / 2;
			return pathSetDistance;
		}


	#if UNITY_EDITOR
		void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				Color col = Color.red;
				Vector3 lineStart, lineEnd;

				foreach (DirectionNode dir in directionNodes)
				{
					if (dir == directionNodes[bestDirectionIndex])
						col = new Color(1f, 0.56f, 0.03f);
					else if (dir.combinedWeight > 0.9f)
						col = Color.green;
					else if (dir.combinedWeight > 0.7f && dir.combinedWeight < 0.9f)
						col = Color.cyan;
					else if (dir.combinedWeight < 0.7f && dir.combinedWeight > 0.5f)
						col = Color.yellow;
					else if (dir.combinedWeight < 0.5f && dir.combinedWeight > 0)
						col = Color.magenta;
					else col = Color.white;
					lineStart = this.transform.position + dir.directionAtAngle.ToVector3();
					lineEnd =
						dir.directionAtAngle.ToVector3(1) * (dir.combinedWeight * CalculationDistance(playerDist)) +
						this.transform.position;
					Debug.DrawLine(lineStart, lineEnd, col);
				}
			}
		}
	#endif
	}
}