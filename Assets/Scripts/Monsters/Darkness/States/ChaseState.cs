using UnityEngine;

namespace DarknessMinion
{

	[CreateAssetMenu(menuName = "Darkness/ChaseState")]
	public class ChaseState : DarkState
	{
		[Range(0.25f, 5)]
		public float pathUpdateRate;
		[Range(0, 10)]
		public int switchTargetDistance;

		public override void InitializeState(Darkness darkController)
		{
			if (darkController.movement.navTarget != null && darkController.movement.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Attack)
			{
				darkController.movement.navTarget.ReleaseTarget();
				DarkEventManager.OnRequestNewTarget(darkController.creationID);
			}
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.movement.ChangeSwitchDistance(switchTargetDistance);
			darkController.movement.StartMovement();
			darkController.movement.UpdateDestinationPath(false);
			//CooldownCallback(darkController);
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
			if(darkController.playerDist <= switchTargetDistance)
				darkController.movement.UpdateDestinationPath(true);
			else darkController.movement.UpdateDestinationPath(false);
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			//darkController.movement.UpdateDestinationPath(false);
			//darkController.sekr.StartPath(darkController.transform.position, darkController.pather.destination);
			//darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.movement.StopMovement();
			base.ExitState(darkController);
			//controller.aIMovement.EndMovement();
			//controller.pather.canSearch = false;
			//controller.pather.pickNextWaypointDist -= 0.5f;
			//controller.pather.endReachedDistance -= 1.0f;
			//controller.sekr.CancelCurrentPathRequest();
		}
	}
}