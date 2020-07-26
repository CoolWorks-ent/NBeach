using System.Collections;
using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/IdleAction")]
    public class IdleAction : Dark_Action
    {
        [SerializeField, Range(0,5)]
        private float idleTime;

        public override void ExecuteAction(DarknessMinion controller) //TODO How am I going to make this halt the darkness from moving while still rotating to look at the player?
        {
            controller.EndMovement();

            //controller.StartActionCooldown(this.GetInstanceID, idleTime);
            //controller.StartCoroutine(IdleTime(controller, idleTime));
        }

    }
}