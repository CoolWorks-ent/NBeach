using System.Collections;
using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/IdleAction")]
    public class IdleAction : Dark_Action
    {
        protected float idleTime;

        public void OnEnable()
        {
            actionType = ActionType.Idle;
        }

        public override void ExecuteAction(DarknessMinion controller)
        {
            controller.EndMovement();
            controller.UpdateAnimator(controller.idleHash);
            //if(!controller.CheckTimedActions(actionType) && !controller.CheckActionsOnCooldown(actionType)) //TODO How can this not be simplified?
            //TimerRequest(controller, executionTime, coolDownTime);

            //TODO how am I going to handle the actual idling activity
            if (!controller.CheckActionsOnCooldown(ActionType.IdleOnly))
                controller.StartCoroutine(IdleExecution(controller));

            Vector3 pDir = Darkness_Manager.Instance.player.position - controller.transform.position; 
            Vector3 dir = Vector3.RotateTowards(controller.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
            controller.transform.rotation = Quaternion.LookRotation(dir);
            //controller.StartActionCooldown(this.GetInstanceID, idleTime);
            //controller.StartCoroutine(IdleTime(controller, idleTime));
        }

        private IEnumerator IdleExecution(DarknessMinion controller)
        {
            controller.AddCooldown(new ActionCooldownInfo(idleTime, ActionType.IdleOnly));
            yield return new WaitForSeconds(idleTime-0.5f);
            controller.AddCooldown(new ActionCooldownInfo(coolDownTime, actionType));
        }
    }
}