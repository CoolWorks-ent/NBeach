using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DarknessMinion
{
	
	[CreateAssetMenu(menuName = "AI/Darkness/State/IdleState")]
	public class IdleState : DarkState
	{
		[Range(0, 5)]
		public float idleTime;

		protected override void FirstTimeSetup()
		{
			stateType = StateType.IDLE;
		}

		public override void InitializeState(Darkness controller)
		{
			Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
			//controller.aIMovement.EndMovement();
			if(controller.navTarget != null)
				DarkEventManager.OnRequestNewTarget(controller.creationID);
			controller.pather.canMove = false;
			controller.animeController.SetTrigger(controller.idleHash);
			controller.AddCooldown(new CooldownInfo(idleTime, CooldownStatus.Idling, CooldownCallback));
			controller.sekr.CancelCurrentPathRequest();
			//base.InitializeState(controller);
		}

		public override void UpdateState(Darkness controller)
		{
			// controller.transform.rotation = Quaternion.LookRotation(dir);
			//CheckTransitions(controller);
			//if (controller.CheckActionsOnCooldown(CooldownStatus.Idling))
			//CheckTransitions(controller);
		}

		public override void MovementUpdate(Darkness controller)
		{
			Vector3 pDir = DarknessManager.Instance.player.position - controller.transform.position;
			Vector3 dir = Vector3.RotateTowards(controller.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
			controller.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
			//Quaternion.RotateTowards(controller.transform.rotation,  DarknessManager.Instance.player.rotation, )
		}

		public override void ExitState(Darkness controller)
		{
			//AI_Manager.Instance.StopCoroutine(IdleTime(controller, idleTime));
			if (controller.animeController != null)
				controller.animeController.ResetTrigger(controller.idleHash);
			controller.ResetCooldowns();
		}

		protected override void CooldownCallback(Darkness controller)
		{
			CheckTransitions(controller);
			controller.AddCooldown(new CooldownInfo(idleTime, CooldownStatus.Idling, CooldownCallback));
			//AI_Manager.Instance.StartCoroutine(IdleTime(controller,idleTime));
			//AI_Manager.Instance.StartCoroutine(IdleTime(controller, idleTime));
		}
	}
}