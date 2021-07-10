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
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.movement.StartMovement();
			darkController.movement.UpdatePathDestination();
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void UpdateState(Darkness darkController)
		{
			CheckTransitions(darkController);
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
		}
	}
}