using System;
using System.Collections.Generic;
using UnityEngine;


namespace Darkness
{
    public abstract class Dark_Action : ScriptableObject
    {
        public enum ActionType {Chase, Idle, Attack, Patrol, Death}

        protected ActionType actionType;

        [SerializeField, Range(0, 10)]
        protected float executionTime, coolDownTime;

        public enum ActionFlags { NavTargetDistClose, EndOfPath, PlayerInAttackRange, PlayerOutOfRange, AttackOnCooldown, ChaseOnCooldown, IdleOnCooldown, NotCurrentlyAttacking}

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
            {ActionFlags.NotCurrentlyAttacking, NotCurrentlyAttacking}
        };

        public struct ActionCooldownInfo 
        {
            public float durationTime;
            public float coolDownTime;
            public Coroutine activeCoroutine;
            //public Coroutine durationRoutine;

            public ActionCooldownInfo(float durTime, float cdTime, Coroutine routine)
            {
                durationTime = durTime;
                coolDownTime = cdTime; 
                activeCoroutine = routine;
                Debug.LogWarning(String.Format("Action Cooldown requested. Starting coroutine {0}", routine));
            }
        }

        public abstract void ExecuteAction(DarknessMinion controller);//make virtual and add time update for how this action has been executing

        protected void TimedActionActivation(DarknessMinion controller, float durationTime, float coolDownTime)
        {
            controller.ProcessActionCooldown(actionType, durationTime, coolDownTime);
        }

        protected Vector3 RandomPoint(Vector3 center, float radiusLower, float radiusUpper)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * Mathf.Sqrt(UnityEngine.Random.Range(radiusLower, radiusUpper));
            return new Vector3(point.x + center.x, center.y, point.y + center.z);
        }

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
            return !controller.CheckTimedActions(ActionType.Attack);
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