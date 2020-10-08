using UnityEngine;
using System.Collections;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "AI/Darkness/State/AttackState")]
    public class AttackState : DarkState
    {
        [Range(1, 3)]
        public int attackSpeedModifier;
        [Range(0, 5)]
        public float attackCooldown;

        [Range(0, 3)]
        public float attackInitiationRange;


        protected override void FirstTimeSetup()
        {
            stateType = StateType.ATTACK;
        }

        public override void InitializeState(Darkness controller)
        {
            Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
            //base.InitializeState(controller);
            //Dark_Event_Manager.OnRequestNewTarget(controller.creationID);
            
            controller.pather.destination = controller.navTarget.srcPosition;
            if (controller.playerDist > attackInitiationRange)
                controller.pather.canMove = true;
            else controller.pather.canMove = false;
            controller.pather.canSearch = true;
        }

        public override void UpdateState(Darkness controller)
        {
            //TODO check if the darkness is facing the player. if not start rotating towards the player
            controller.pather.destination = controller.navTarget.navPosition;
            if (controller.playerDist < attackInitiationRange && !controller.attacked)
            {
                //controller.attacked = true;
                controller.animeController.SetTrigger(controller.attackHash);
                controller.pather.canMove = false;
                controller.AddCooldown(new CooldownInfo(attackCooldown, CooldownStatus.Attacking, CooldownCallback));
                controller.darkHitBox.enabled = true;
                controller.attacked = true;
                //if(controller.animeController.animation.)
                //controller.StartCoroutine(controller.AttackCooldown(attackCooldown, controller.idleHash));
            }
            /*else 
            {
                controller.pather.canMove = true;
            }*/
            CheckTransitions(controller);
        }

        public override void MovementUpdate(Darkness controller)
        {

        }

        public override void ExitState(Darkness controller)
        {
            controller.pather.endReachedDistance -= 1.0f;
            //controller.attacked = false;
            //controller.animeController.SetBool(controller.attackAfterHash, true);
        }

        private void AttackInitiated(Darkness controller)
        {

        }

        protected override void CooldownCallback(Darkness controller)
        {
            //animeController.SetTrigger(animationID);
            controller.attacked = false;
            controller.darkHitBox.enabled = false;
        }
    }
}