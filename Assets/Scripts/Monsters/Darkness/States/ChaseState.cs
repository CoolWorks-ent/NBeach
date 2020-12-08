using UnityEngine;

namespace DarknessMinion
{

	[CreateAssetMenu(menuName = "Darkness/ChaseState")]
	public class ChaseState : DarkState
	{
		[Range(0.25f, 5)]
		public float pathUpdateRate;
		//[Range(0, 10)]
		//public float switchToAttackDist;

		//public ChaseState(Darkness dControl) : base(dControl){ }

		public override void InitializeState(Darkness darkController)
		{
			//Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
			if (darkController.navTarget != null && darkController.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Attack)
			{
				darkController.navTarget.ReleaseTarget();
				DarkEventManager.OnRequestNewTarget(darkController.creationID);
				//controller.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
			}
			/*else if (controller.navTarget == null)
			{
				Debug.LogWarning(string.Format("nav target {0} not initialised for {1}", controller.navTarget, controller.creationID));
				DarkEventManager.OnRequestNewTarget(controller.creationID);
			}*/

			//controller.animeController.SetTrigger(controller.chaseTrigHash);
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.pather.destination = darkController.navTarget.transformPosition;
			darkController.pather.canMove = true;
			//darkController.pather.canSearch = true;
			darkController.pather.repathRate = 0;
			CooldownCallback(darkController);
			//darkController.swtichDist = switchToAttackDist;
			/*controller.aIMovement.CreatePath(controller.Target.position);
			controller.aIMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
			controller.aIMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
			controller.aIMovement.moving = true;
			controller.aIRichPath.endReachedDistance = stopDist;*/
		}

		public override void UpdateState(Darkness darkController)
		{
			//controller.aIMovement.UpdatePath(controller.Target.position);
			CheckTransitions(darkController);
		}

		public override void MovementUpdate(Darkness darkController)
		{
			darkController.pather.destination = darkController.navTarget.transformPosition;
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			darkController.pather.destination = darkController.navTarget.transformPosition;
			darkController.sekr.StartPath(darkController.transform.position, darkController.pather.destination);
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.navTarget.ReleaseTarget();
			darkController.pather.canMove = false;
			darkController.sekr.CancelCurrentPathRequest();
			base.ExitState(darkController);
			//controller.aIMovement.EndMovement();
			//controller.pather.canSearch = false;
			//controller.pather.pickNextWaypointDist -= 0.5f;
			//controller.pather.endReachedDistance -= 1.0f;
			//controller.sekr.CancelCurrentPathRequest();
		}
	}
}