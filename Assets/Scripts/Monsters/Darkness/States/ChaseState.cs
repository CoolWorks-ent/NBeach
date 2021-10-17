using UnityEngine;
using DarknessMinion.Movement;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "Darkness/ChaseState")]
	public class ChaseState : DarkState
	{
		[SerializeField, Range(0.25f, 5)]
		protected float pathUpdateRateFar, pathUpdateRateClose;
		
		[SerializeField, Range(2, 15)]
		private float closenessRange;

		public override void InitializeState(Darkness darkController)
		{
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Chase);
			darkController.movement.StartMovement();
			CooldownCallback(darkController);
			//darkController.AddCooldown(new CooldownInfo(UpdateRate(darkController.PlayerDistance()), CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void UpdateState(Darkness darkController)
		{
			CheckTransitions(darkController);
		}

		public override void MovementUpdate(Darkness darkController)
		{
			darkController.movement.MoveBody();
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			Vector3 direction;
			DarknessMovement darkMove = darkController.movement;
			AttackZone atkZone = AttackZoneManager.Instance.playerAttackZone;
			if (atkZone.InTheZone(darkMove.ConvertToVec2(darkMove.transform.position)))
				direction = (atkZone.AttackPoint() - darkMove.transform.position).normalized;
			else direction = (atkZone.attackZoneOrigin - darkController.transform.position).normalized;
			darkController.movement.UpdatePathDestination(direction);
			darkController.AddCooldown(new CooldownInfo(UpdateRate(darkController.PlayerDistance()), CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void ExitState(Darkness darkController)
		{
			darkController.movement.StopMovement();
			base.ExitState(darkController);
		}

		private float UpdateRate(float playerDist)
		{
			if(playerDist < closenessRange)
				return pathUpdateRateClose;
			return pathUpdateRateFar;
		}
	}
}