using UnityEngine;
using UnityEditor;

namespace DarknessMinion
{

	[CreateAssetMenu(menuName = "AI/Darkness/State/ChaseState")]
	public class ChaseState : DarkState
	{
		[Range(0.25f, 5)]
		public float pathUpdateRate;
		protected override void FirstTimeSetup()
		{
			stateType = StateType.CHASING;
		}

		public override void InitializeState(Darkness controller)
		{
			Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
			if (controller.navTarget != null && controller.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Attack)
			{
				controller.navTarget.ReleaseTarget();
				DarkEventManager.OnRequestNewTarget(controller.creationID);
				//controller.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
			}
			/*else if (controller.navTarget == null)
			{
				Debug.LogWarning(string.Format("nav target {0} not initialised for {1}", controller.navTarget, controller.creationID));
				DarkEventManager.OnRequestNewTarget(controller.creationID);
			}*/

			controller.animeController.SetTrigger(controller.chaseHash);
			controller.pather.destination = controller.navTarget.navPosition;
			controller.pather.canMove = true;
			controller.pather.canSearch = true;
			/*controller.aIMovement.CreatePath(controller.Target.position);
			controller.aIMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
			controller.aIMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
			controller.aIMovement.moving = true;
			controller.aIRichPath.endReachedDistance = stopDist;*/
		}

		public override void UpdateState(Darkness controller)
		{
			//controller.aIMovement.UpdatePath(controller.Target.position);
			CheckTransitions(controller);
		}

		public override void MovementUpdate(Darkness controller)
		{
			controller.pather.destination = controller.navTarget.navPosition;
		}

		protected override void CooldownCallback(Darkness controller)
		{
			controller.pather.destination = controller.navTarget.navPosition;
			controller.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness controller)
		{
			controller.navTarget.ReleaseTarget();
			//controller.aIMovement.EndMovement();
			controller.pather.canMove = false;
			//controller.pather.canSearch = false;
			//controller.pather.pickNextWaypointDist -= 0.5f;
			//controller.pather.endReachedDistance -= 1.0f;
			//controller.sekr.CancelCurrentPathRequest();
		}
	}
}