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

        [Range(0, 10)]
        public float attackInitiationRange, attackSwitchTargetDistance;

        protected override void FirstTimeSetup()
        {
            stateType = StateType.ATTACK;
        }

        public override void InitializeState(Darkness controller)
        {
            //Debug.LogWarning(string.Format("Darkness {0} has entered {1} State at {2}", controller.creationID, this.name, Time.deltaTime));
            //base.InitializeState(controller);
            //DarkEventManager.OnRequestNewTarget(controller.creationID);
            
            //controller.pather.destination = controller.navTarget.srcPosition;
            if (controller.playerDist > attackInitiationRange)
                controller.pather.canMove = true;
            else controller.pather.canMove = false;
            controller.pather.canSearch = true;
            controller.pather.endReachedDistance = attackInitiationRange;
        }

        public override void UpdateState(Darkness controller)
        {
            //TODO check if the darkness is facing the player. if not start rotating towards the player
            CheckTransitions(controller);
        }

        public override void MovementUpdate(Darkness controller) //TODO figure out how I should stop and rotate towards the player. Where should these checks happen?
        {
            if(controller.playerDist > attackSwitchTargetDistance)//if(!controller.pather.reachedEndOfPath)
                controller.pather.destination = controller.navTarget.navPosition;
            else 
            {
                controller.pather.destination = controller.navTarget.closeToSrcPosition;
                RaycastHit info;
                if(Physics.Raycast(controller.transform.position, controller.transform.forward*attackInitiationRange, out info, attackInitiationRange, controller.mask)) //controller.playerDist < attackInitiationRange && 
                {
                    Debug.Log("Hit collider: " + info.collider.name);
                    if(info.collider.name == "Player")
                    {
                        if(!controller.CheckActionsOnCooldown(DarkState.CooldownStatus.Attacking))
                        {
                        controller.pather.canMove = false;
                        controller.animeController.SetTrigger(controller.attackHash);
                        
                        controller.AddCooldown(new CooldownInfo(attackCooldown, CooldownStatus.Attacking, CooldownCallback));
                        controller.AddCooldown(new CooldownInfo(attackCooldown/4, CooldownStatus.Idling, IdleAnimation));
                        controller.darkHitBox.enabled = true;
                        controller.attacked = true;
                        }
                    }
                }
                else if(controller.pather.reachedEndOfPath)
                {
                    //controller.pather.canMove = false;
                    controller.animeController.SetTrigger(controller.idleHash);
                    Vector3 pDir = DarknessManager.Instance.player.position - controller.transform.position;
                    Vector3 dir = Vector3.RotateTowards(controller.transform.forward, pDir, 2.0f * Time.deltaTime, 0.1f);
                    controller.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
                }
            }
        }

        public override void ExitState(Darkness controller)
        {
            controller.pather.endReachedDistance -= 1.0f;
            //controller.attacked = false;
            //controller.animeController.SetBool(controller.attackAfterHash, true);
        }

        private void IdleAnimation(Darkness controller)
        {
            controller.animeController.SetTrigger(controller.idleHash);
        }

        protected override void CooldownCallback(Darkness controller)
        {
            controller.darkHitBox.enabled = false;
            //animeController.SetTrigger(animationID);
            controller.attacked = false;
            controller.pather.canMove = true;
        }
    }
}