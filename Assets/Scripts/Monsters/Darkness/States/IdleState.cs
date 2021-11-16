using UnityEngine;

namespace Darkness.States
{
	
	[CreateAssetMenu(menuName = "Darkness/IdleState")]
	public class IdleState : DarkState
	{
		[SerializeField, Range(0, 5)]
		private float idleTime;

		public bool waitFullIdleTime;

		public override void InitializeState(DarknessController darkController)
		{

			darkController.steering.ResetMovement();
	
			darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Idle);
			darkController.AssignCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
		}

		public override void UpdateState(DarknessController darkController)
		{
			if(!waitFullIdleTime)
				CheckTransitions(darkController);
		}

		public override void MovementUpdate(DarknessController darkController)
		{
			darkController.steering.movementController.RotateTowardsDirection(darkController.steering.Target.position);
		}

		public override void ExitState(DarknessController darkController)
		{
			darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Idle);
			base.ExitState(darkController);
		}

		protected override void CooldownCallback(DarknessController darkController)
		{
			CheckTransitions(darkController);
			darkController.AssignCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
		}
	}
}