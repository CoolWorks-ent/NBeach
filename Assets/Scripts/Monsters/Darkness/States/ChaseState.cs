using UnityEngine;
using Darkness.Movement;

namespace Darkness.States
{
	[CreateAssetMenu(menuName = "Darkness/ChaseState")]
	public class ChaseState : DarkState
	{
		[SerializeField, Range(0.25f, 5)]
		protected float pathUpdateRateFar, pathUpdateRateClose;
		
		[SerializeField, Range(2, 15)]
		private float closenessRange;

		public override void InitializeState(DarknessController darkController)
		{
			darkController.ChangeAnimation(DarknessController.DarkAnimationStates.Chase);
			CooldownCallback(darkController);
			//darkController.AddCooldown(new CooldownInfo(UpdateRate(darkController.PlayerDistance()), CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		public override void UpdateState(DarknessController darkController)
		{
			CheckTransitions(darkController);
		}

		public override void MovementUpdate(DarknessController darkController)
		{
			//TODO call a movement function on the 
			darkController.steering.FindNearbyAvoidables();
		}

		protected override void CooldownCallback(DarknessController darkController)
		{
			Vector2 direction;
			AISteering steering = darkController.steering;
			AttackZone atkZone = AttackZoneManager.Instance.playerAttackZone;
			if (atkZone.InTheZone(darkController.transform.position.ToVector2()))
				direction = steering.Seek(atkZone.AttackPoint().ToVector2());
			else direction = steering.Seek(atkZone.attackZoneOrigin.ToVector2());
			direction += steering.AvoidObstacles(1.5f, 1.25f) * 1.15f;
			direction += steering.AvoidAgent();
			steering.SetMovementDirection(direction);
			//darkController.steering.DetermineBestDirection(destination);
			darkController.AssignCooldown(new CooldownInfo(UpdateRate(darkController.PlayerDistance()), CooldownInfo.CooldownStatus.Moving, CooldownCallback));
		}

		protected void CheckCooldown()
		{
			
		}
		
		private float UpdateRate(float playerDist)
		{
			if(playerDist < closenessRange)
				return pathUpdateRateClose;
			return pathUpdateRateFar;
		}
	}
}