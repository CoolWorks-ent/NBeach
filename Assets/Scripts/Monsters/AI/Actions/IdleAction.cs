using System.Collections;
using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/IdleAction")]
    public class IdleAction : Dark_Action
    {

        public void OnEnable()
        {
            actionType = ActionType.Idle;
        }

        public override void ExecuteAction(DarknessMinion controller) //TODO How am I going to make this halt the darkness from moving while still rotating to look at the player?
        {
            controller.EndMovement();
            controller.UpdateAnimator(controller.idleHash);
            if(!controller.CheckTimedActions(actionType) && !controller.CheckActionsOnCooldown(actionType))
                TimedActionActivation(controller, executionTime, coolDownTime);

            Vector3 pDir = Darkness_Manager.Instance.player.position - controller.transform.position; 
            Vector3 dir = Vector3.RotateTowards(controller.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
            controller.transform.rotation = Quaternion.LookRotation(dir);
            //controller.StartActionCooldown(this.GetInstanceID, idleTime);
            //controller.StartCoroutine(IdleTime(controller, idleTime));
        }

    }
}