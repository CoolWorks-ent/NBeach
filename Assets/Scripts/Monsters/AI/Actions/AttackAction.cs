using UnityEngine;
using System.Collections;

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

        public void OnEnable()
        {
            actionType = ActionType.Attack;
        }

        public override void ExecuteAction(DarknessMinion controller)
        {

            //Face player then chomp

            if(controller.navTarget.navTargetTag != DarknessMinion.NavTargetTag.Attack)
                Darkness_Manager.OnRequestNewTarget(controller.creationID, DarknessMinion.NavTargetTag.Attack);
            //TODO determine if it's not already moving
            if(controller.playerDist < attackInitiationRange)
            {
                controller.EndMovement();
                //TODO add utility function on the Dark_State level for rotating the darkness to face the player
                //If the rotation is within a certain range say +/-5 degrees then try a sphere cast to see if I can attack
                //if that returns the player chomp em
                //Darkness_Manager.OnRequestNewTarget(controller.creationID, DarknessMinion.NavTargetTag.Player);
                controller.UpdateAnimator(controller.attackHash);
                TimedActionActivation(controller, attackHitboxTime, attackCooldown);
                controller.StartCoroutine(AttackActivation(controller));
                
            } 
            else controller.ResumeMovement();
        }

        public IEnumerator AttackActivation(DarknessMinion controller)
        {
            //attackOnCooldown = true;
            controller.darkHitBox.enabled = true;
            //animeController.SetTrigger(animationID);
            yield return new WaitForSeconds(attackHitboxTime);
            //attackOnCooldown = false;
            controller.darkHitBox.enabled = false;
        }
    }      
}