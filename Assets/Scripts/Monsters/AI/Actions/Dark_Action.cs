using System;
using System.Collections.Generic;
using UnityEngine;


namespace Darkness
{
    public abstract class Dark_Action : ScriptableObject
    {
        public enum ActionType {Chase, Idle, IdleOnly, Attack, Patrol, Death}

        protected ActionType actionType;

        [SerializeField, Range(0, 10)]
        protected float coolDownTime;

        public enum ActionFlags { NavTargetDistClose, EndOfPath, PlayerInAttackRange, PlayerOutOfRange, AttackOnCooldown, ChaseOnCooldown, IdleOnCooldown, NotCurrentlyAttacking, PauseForIdle}

        [SerializeField]
        private ActionFlags[] Conditions;

        private Dictionary<ActionFlags,Func<DarknessMinion, bool>> ActionFlagCheck = new Dictionary<ActionFlags, Func<DarknessMinion, bool>>
        {
            {ActionFlags.PlayerInAttackRange, PlayerInAttackRange},
            {ActionFlags.NavTargetDistClose, NavTargetDistCloseCheck},
            {ActionFlags.EndOfPath, EndOfPathCheck},
            {ActionFlags.PlayerOutOfRange, PlayerOutOfRange},
            {ActionFlags.AttackOnCooldown, AttackOnCooldown},
            {ActionFlags.IdleOnCooldown, IdleOnCooldown},
            {ActionFlags.ChaseOnCooldown, ChaseOnCooldown},
            {ActionFlags.NotCurrentlyAttacking, NotCurrentlyAttacking},
            {ActionFlags.PauseForIdle, PauseIdlingCheck}
        };

        public struct ActionCooldownInfo 
        {
            public ActionType acType { get; private set; }
            public float remainingTime;
            public float coolDownTime;
            //public Coroutine durationRoutine;

            public ActionCooldownInfo(float cdTime, ActionType acT)
            {
                remainingTime = cdTime;
                coolDownTime = cdTime;
                acType = acT;
            }

            public bool CheckTimerComplete(float time)
            {
                remainingTime = Mathf.Max(remainingTime - time, 0);
                if (remainingTime == 0)
                    return true;
                else return false;
            }
        }

        public abstract void ExecuteAction(DarknessMinion controller);//make virtual and add time update for how this action has been executing

        /*protected void TimerRequest(DarknessMinion controller, float durationTime, float coolDownTime)
        {
            ActionCooldownInfo actionCooldownInfo = new ActionCooldownInfo(coolDownTime, this.actionType);
            controller.AddCooldown(actionCooldownInfo);
        }*/

        public bool ConditionsMet(DarknessMinion controller)
        {
            if(Conditions.Length > 0)
            {
                foreach(ActionFlags actCond in Conditions)
                {
                    if(!CheckFlag(actCond, controller))
                        return false;
                }
            }
            return true;
        } 

        private bool CheckFlag(ActionFlags fName, DarknessMinion controller)
        {   
            if(controller != null)
                return ActionFlagCheck[fName].Invoke(controller);
            else return false;
        }

        private static bool PauseIdlingCheck(DarknessMinion controller)
        {
            return controller.CheckActionsOnCooldown(ActionType.IdleOnly);
        }

        private static bool EndOfPathCheck(DarknessMinion controller)
        {
            return controller.reachedEndOfPath;
        }

        private static bool ChaseOnCooldown(DarknessMinion controller)
        {
            return controller.CheckActionsOnCooldown(ActionType.Chase);
        }

        private static bool AttackOnCooldown(DarknessMinion controller)
        {
            return controller.CheckActionsOnCooldown(ActionType.Attack);
        }

        private static bool NotCurrentlyAttacking(DarknessMinion controller)
        {
            return !controller.darkHitBox.enabled;
        }

        private static bool IdleOnCooldown(DarknessMinion controller)
        {
            return controller.CheckActionsOnCooldown(ActionType.Idle);
        }

        private static bool PatrolOnCooldown(DarknessMinion controller)
        {
            return controller.CheckActionsOnCooldown(ActionType.Patrol);
        }

        private static bool PlayerInAttackRange(DarknessMinion controller) 
        {
            if(controller.playerDist <= controller.switchTargetDistance) 
            {
                return true;
            }
            else return false;
        }

        private static bool NavTargetDistCloseCheck(DarknessMinion controller) 
        {
            if(controller.targetDistance < controller.switchTargetDistance || EndOfPathCheck(controller))
            {
                return true;
            }
            else return false;
        }

        private static bool PlayerOutOfRange(DarknessMinion controller) 
        {
            if(controller.playerDist > controller.switchTargetDistance)
            {
                return true;
            }
            else return false;
        }
    }
}