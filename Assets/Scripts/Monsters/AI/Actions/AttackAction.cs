using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/AttackAction")]
    public class AttackAction : Dark_Action
    {
        [SerializeField]
        private float attackInitiationRange; 

        [SerializeField, Range(0,5)]
        private float attackHitboxTime;

        [SerializeField, Range(0,5)]
        private float attackCooldown;

        public override void ExecuteAction(DarknessMinion controller)
        {
            
            if(controller.attackEnded)
            {
                controller.darkHitBox.enabled = false;
                RequestActionCooldown(controller, attackCooldown, ActionCooldownType.Attack);
            }
            if(controller.playerDist < attackInitiationRange)
            {
                //TODO add utility function on the Dark_State level for rotating the darkness to face the player
                //If the rotation is within a certain range say +/-5 degrees then try a sphere cast to see if I can attack
                //if that returns the player chomp em
                Darkness_Manager.OnRequestNewTarget(controller.creationID, DarknessMinion.NavTargetTag.Player);
                controller.ResumeMovement();
            }
            else 
            {
                Darkness_Manager.OnRequestNewTarget(controller.creationID, DarknessMinion.NavTargetTag.Attack);
                if(!controller.attackActive)
                {
                    RequestActionCooldown(controller, attackHitboxTime, ActionCooldownType.AttackActive);
                    controller.darkHitBox.enabled = true;
                    controller.UpdateAnimator(animationType);
                }
            }
        }
    }
}