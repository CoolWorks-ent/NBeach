using UnityEngine;
using Pathfinding;

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

		[HideInInspector]
		public float playerDist { get; private set; }
		public float navTargetDist { get; private set; }

		[HideInInspector]
		public NavigationTarget navTarget;
		[HideInInspector]
		public Transform player;

		private Seeker sekr;
		private AIPath pather;

		private DarknessSteering steering;

		private Vector3[] seekMap, avoidMap, calculationMap;
		private float[] angleMap;
		
		void Awake()
		{
			steering = new DarknessSteering();
			sekr = GetComponent<Seeker>();
			pather = GetComponent<AIPath>();

			calculationMap = new Vector3[8];
			seekMap = new Vector3[calculationMap.Length];
			avoidMap = new Vector3[calculationMap.Length];
			angleMap = new float[calculationMap.Length];

			angleMap[0] = 90 * Mathf.Deg2Rad;
			angleMap[1] = 120 * Mathf.Deg2Rad;
			angleMap[2] = 140 * Mathf.Deg2Rad;
			angleMap[3] = 160 * Mathf.Deg2Rad;
			angleMap[4] = 70 * Mathf.Deg2Rad;
			angleMap[5] = 50 * Mathf.Deg2Rad;
			angleMap[6] = 30 * Mathf.Deg2Rad;
			angleMap[7] = 270 * Mathf.Deg2Rad;
		}

		void Start()
		{
			//sekr.pathCallback += PathComplete;
			//bProvider = new Blocker();
			DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
		}

		void LateUpdate() //for testing only
        {
			GenerateVectorsatAngles();
        }

		public void DistanceEvaluation(Vector3 location)
		{
			playerDist = Vector2.Distance(ConvertToVec2(transform.position), ConvertToVec2(location));
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

		public void GenerateVectorsatAngles()
        {
			for(int i = 0; i < calculationMap.Length; i++)
            {
				calculationMap[i] = new Vector3(Mathf.Cos(angleMap[i]), 0.1f, Mathf.Sin(angleMap[i]));
            }
        }

		public void PathChooser()
        {

        }

		private void PathsGenerator()
        {
			//Generate vectors in several directions in a circle around the Darkness
			//I want points at certain angles all around the Darkness and I want to save those to an array
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
			foreach(Vector3 vec in calculationMap)
            {
				Debug.DrawLine(this.transform.position, vec + this.transform.position, Color.green);
            }
        }

		void OnDestroy()
		{
			DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation;
		}
	}
}