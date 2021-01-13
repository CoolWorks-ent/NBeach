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

		private Seeker sekr;
		private AIPath pather;

		private DarknessSteering steering;

		void Awake()
		{
			steering = new DarknessSteering();
			sekr = GetComponent<Seeker>();
			pather = GetComponent<AIPath>();
		}

		void Start()
		{
			//sekr.pathCallback += PathComplete;
			//bProvider = new Blocker();
			DarkEventManager.UpdateDarknessDistance += DistanceEvaluation;
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

		void OnDestroy()
		{
			DarkEventManager.UpdateDarknessDistance -= DistanceEvaluation;
		}
	}
}