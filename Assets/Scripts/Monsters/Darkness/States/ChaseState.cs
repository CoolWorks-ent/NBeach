using UnityEngine;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "Darkness/ChaseState")]
	public class ChaseState : DarkState
	{
		[Range(0.25f, 5)]
		public float pathUpdateRate;

		public override void InitializeState(Darkness darkController)
		{
			/*if (darkController.movement.navTarget != null && darkController.movement.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Attack)
			{
				//TODO check target zone instead of requesting a new point
			}*/
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.movement.StartMovement();
			darkController.movement.UpdatePathDestination();
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
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
			/*if (darkController.movement.playerDist <= darkController.movement.switchTargetDistance)
				darkController.movement.UpdateDestinationPath(true);
			else darkController.movement.UpdateDestinationPath(false);*/
		}

		public override void MovementUpdate(Darkness darkController)
		{	
			//darkController.movement.MoveDarkness();
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			darkController.movement.UpdatePathDestination();
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
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