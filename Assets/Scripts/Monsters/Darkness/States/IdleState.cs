using UnityEngine;

namespace DarknessMinion
{
	
	[CreateAssetMenu(menuName = "Darkness/IdleState")]
	public class IdleState : DarkState
	{
		[SerializeField, Range(0, 5)]
		private float idleTime;

		public bool waitFullIdleTime;

		//public IdleState(Darkness dControl) : base(dControl){ }

		public override void InitializeState(Darkness darkController)
		{
			//Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
			//controller.aIMovement.EndMovement();
			darkController.movement.StopMovement();
			if(darkController.movement.navTarget != null)
			{
				darkController.movement.CreateDummyNavTarget(DarknessManager.Instance.groundLevel);
				//darkController.sekr.CancelCurrentPathRequest();
			}
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
			//controller.animeController.SetTrigger(controller.idleTrigHash);

			//if(!controller.IsAnimationPlaying(Darkness.DarkAnimationStates.Spawn))
			//	controller.ChangeAnimation(Darkness.DarkAnimationStates.Idle);

			darkController.AddCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
			
			//base.InitializeState(controller);
		}

		public override void UpdateState(Darkness darkController)
		{
			if(!waitFullIdleTime)
				CheckTransitions(darkController);
		}

		public override void MovementUpdate(Darkness darkController)
		{
			darkController.movement.RotateTowardsPlayer();
			//Quaternion.RotateTowards(controller.transform.rotation,  DarknessManager.Instance.player.rotation, )
		}

		public override void ExitState(Darkness darkController)
		{
			//AI_Manager.Instance.StopCoroutine(IdleTime(controller, idleTime));
			darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
				//controller.animeController.ResetTrigger(controller.idleTrigHash);
			base.ExitState(darkController);
		}

		protected override void CooldownCallback(Darkness darkController)
		{
			CheckTransitions(darkController);
			darkController.AddCooldown(new CooldownInfo(idleTime, CooldownInfo.CooldownStatus.Idling, CooldownCallback));
			//AI_Manager.Instance.StartCoroutine(IdleTime(controller,idleTime));
			//AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
		}
	}
}