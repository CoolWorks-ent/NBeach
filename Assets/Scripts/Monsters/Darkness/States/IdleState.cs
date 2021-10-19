using UnityEngine;

namespace DarknessMinion
{
	
	[CreateAssetMenu(menuName = "Darkness/IdleState")]
	public class IdleState : DarkState
	{
		[SerializeField, Range(0, 5)]
		private float idleTime;

		public bool waitFullIdleTime;

		public override void InitializeState(Darkness darkController)
		{

			darkController.movement.StopMovement();
	
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
			darkController.AssignCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
		}

		public override void UpdateState(Darkness darkController)
		{
			if(!waitFullIdleTime)
				CheckTransitions(darkController);
		}

		public override void MovementUpdate(Darkness darkController)
		{
			darkController.movement.RotateTowardsPlayer();
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
			base.ExitState(darkController);
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			CheckTransitions(darkController);
			darkController.AssignCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
		}
	}
}