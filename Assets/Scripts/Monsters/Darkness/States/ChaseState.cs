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
			if (darkController.navTarget != null && darkController.navTarget.navTargetTag != NavigationTarget.NavTargetTag.Attack)
			{
				darkController.navTarget.ReleaseTarget();
				DarkEventManager.OnRequestNewTarget(darkController.creationID);
			}
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.pather.destination = darkController.navTarget.GetPosition();
			darkController.pather.canMove = true;
			darkController.pather.canSearch = true;
			darkController.pather.repathRate = 1f;
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
			darkController.pather.destination = darkController.navTarget.GetPosition();
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			darkController.pather.destination = darkController.navTarget.GetPosition();
			darkController.sekr.StartPath(darkController.transform.position, darkController.pather.destination);
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.navTarget.ReleaseTarget();
			darkController.pather.canMove = false;
			darkController.pather.canSearch = false;
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