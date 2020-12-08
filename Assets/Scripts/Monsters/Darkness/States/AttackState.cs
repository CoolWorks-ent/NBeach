using UnityEngine;
using System.Collections;

namespace DarknessMinion
{

    [CreateAssetMenu(menuName = "Darkness/AttackState")]
    public class AttackState : DarkState
    {
        [Range(0, 5)]
        public float attackCooldown;

        //[Range(0, 10)]
        //public float attackInitiationRange, attackSwitchTargetDistance;

        //public AttackState(Darkness dControl) : base(dControl){ }

        public override void InitializeState(Darkness darkController)
        {   
            //controller.pather.destination = controller.navTarget.srcPosition;
            /*if (darkController.playerDist > attackInitiationRange)
                darkController.pather.canMove = true;
            else darkController.pather.canMove = false;*/
            //darkController.pather.canSearch = false;
            darkController.pather.canMove = false;
            //darkController.pather.endReachedDistance = attackInitiationRange;
        }

        public override void UpdateState(Darkness darkController)
        {
            //TODO check if the darkness is facing the player. if not start rotating towards the player
            CheckTransitions(darkController);
        }

        public override void MovementUpdate(Darkness darkController) //TODO figure out how I should stop and rotate towards the player. Where should these checks happen?
        {
            darkController.pather.destination = darkController.navTarget.closeToSrcPosition;
            RaycastHit info;
            if(Physics.Raycast(darkController.transform.position, darkController.transform.forward*5, out info, 5, darkController.mask)) //controller.playerDist < attackInitiationRange && 
            {
                Debug.Log("Hit collider: " + info.collider.name);
                if(info.collider.name == "Player")
                {
                    if(!darkController.CheckActionsOnCooldown(DarkState.CooldownStatus.Attacking))
                    {
                        darkController.ChangeAnimation(Darkness.DarkAnimationStates.Attack);
                        //controller.animeController.SetTrigger(controller.attackTrigHash);
                        
                        darkController.AddCooldown(new CooldownInfo(attackCooldown, CooldownStatus.Attacking, CooldownCallback));
                        darkController.AddCooldown(new CooldownInfo(darkController.CurrentAnimationLength(), CooldownStatus.Idling, IdleAnimation));
                        darkController.darkHitBox.enabled = true;
                        darkController.attacked = true;
                    }
                }
            }
            else
            {
                //controller.pather.canMove = false;
                //controller.animeController.SetTrigger(controller.idleTrigHash);
                if(darkController.IsAnimationPlaying(Darkness.DarkAnimationStates.Attack))
                    darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
                Vector3 pDir = DarknessManager.Instance.PlayerToDirection(darkController.transform.position);
                Vector3 dir = Vector3.RotateTowards(darkController.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
                darkController.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            }
        }

        public override void ExitState(Darkness darkController)
        {
            darkController.pather.endReachedDistance -= 1.0f;
            base.ExitState(darkController);
            //controller.attacked = false;
            //controller.animeController.SetBool(controller.attackAfterHash, true);
        }

        private void IdleAnimation(Darkness darkController)
        {
            darkController.ChangeAnimation(Darkness.DarkAnimationStates.Idle);
            //controller.animeController.SetTrigger(controller.idleTrigHash);
        }

        protected override void CooldownCallback(Darkness darkController)
        {
            darkController.darkHitBox.enabled = false;
            //animeController.SetTrigger(animationID);
            darkController.attacked = false;
            darkController.pather.canMove = true;
        }
    }
}