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
			darkController.movement.UpdatePathDestination(AttackZoneManager.Instance.RequestZoneTarget(darkController.creationID)); //pass attackZone
			darkController.movement.StartMovement(); 
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void UpdateState(Darkness darkController)
		{
			CheckTransitions(darkController);
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			//TODO check if attackZone valid, if not request new zone
			darkController.movement.UpdatePathDestination(AttackZoneManager.Instance.RequestZoneTarget(darkController.creationID)); 
			darkController.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.movement.StopMovement();
			AttackZoneManager.Instance.DeAllocateZone(darkController.creationID);
			base.ExitState(darkController);
		}
	}
}