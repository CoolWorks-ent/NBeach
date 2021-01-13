using UnityEngine;
using System.Collections;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "Darkness/AttackState")]
    public class AttackState : DarkState
    {
        //[Range(0, 5)]
        //public float attackCooldown;

        [Range(0, 10)]
        public float attackInitiationRange;

        //public AttackState(Darkness dControl) : base(dControl){ }

        public override void InitializeState(Darkness darkController)
        {   
            //controller.pather.destination = controller.navTarget.srcPosition;
            /*if (darkController.playerDist > attackInitiationRange)
                darkController.pather.canMove = true;
            else darkController.pather.canMove = false;*/
            //darkController.pather.canSearch = false;
            //darkController.pather.canMove = false;
            //darkController.pather.endReachedDistance = attackInitiationRange;
            darkController.movement.StopMovement();
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
            darkController.SetAttackDistance(attackInitiationRange);
        }

        public override void UpdateState(Darkness darkController)
        {
            //TODO check if the darkness is facing the player. if not start rotating towards the player
            CheckTransitions(darkController);
        }

        public override void MovementUpdate(Darkness darkController) //TODO figure out how I should stop and rotate towards the player. Where should these checks happen?
        {
            //darkController.pather.destination = darkController.navTarget.closeToSrcPosition;
            
            if(!darkController.IsAnimationPlaying(Darkness.DarkAnimationStates.Attack))
            {
                if(Physics.Raycast(darkController.transform.position, darkController.transform.forward*5, out darkController.rayHitInfo, 5, darkController.mask)) //controller.playerDist < attackInitiationRange && 
                {
                    Debug.Log("Hit collider: " + darkController.rayHitInfo.collider.name);
                    if(darkController.rayHitInfo.collider.name == "Player")
                    {
                        if(!darkController.CheckActionsOnCooldown(CooldownInfo.CooldownStatus.Attacking))
                        {
                            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Attack);
                            //controller.animeController.SetTrigger(controller.attackTrigHash);
                            
                            darkController.AddCooldown(new CooldownInfo(2f, CooldownInfo.CooldownStatus.Attacking, CooldownCallback));
                            //darkController.AddCooldown(new CooldownInfo(darkController.CurrentAnimationLength(), CooldownStatus.Idling, IdleAnimation));
                            darkController.darkHitBox.enabled = true;
                            //darkController.attacked = true;
                        }
                    }
                }
                else darkController.movement.RotateTowardsPlayer();
            }
        }

        public override void ExitState(Darkness darkController)
        {
            base.ExitState(darkController);
            //controller.attacked = false;
            //controller.animeController.SetBool(controller.attackAfterHash, true);
        }

        /*private void IdleAnimation(Darkness darkController)
        {
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
            //controller.animeController.SetTrigger(controller.idleTrigHash);
        }*/

        protected override void CooldownCallback(Darkness darkController)
        {
            darkController.darkHitBox.enabled = false;
            //animeController.SetTrigger(animationID);
            //darkController.attacked = false;
            darkController.movement.StopMovement();
        }
    }
}