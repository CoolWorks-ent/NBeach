using System;
using System.Collections.Generic;
using UnityEngine;


namespace Darkness
{
    public abstract class Dark_Action : ScriptableObject
    {
        public enum AnimationType {Chase, Idle, None, Attack}

        [SerializeField]
        ///<summary>Assigned in inspector. Determines the animation clip used to play during the action</summary> 
        protected AnimationType animationType; 
        //public bool hasTimer;

        //[SerializeField]
        //public float executionTime;

        public enum ActionFlags {AttackOnCooldown, AttackOffCooldown, NavTargetDistClose, EndOfPath, MovementOnCooldown, IdleOnCooldown, IdleOffCooldown, PlayerInAttackRange, PlayerOutOfRange}
        public enum ActionCooldownType {Attack, AttackActive, Idle, Movement}

        public struct ActionCooldownInfo 
        {
            public ActionCooldownType acType;
            public float initialTime;
            public float coolDownTime;

            public ActionCooldownInfo(ActionCooldownType act,float initTime, float cdTime)
            {
                acType = act;
                initialTime = initTime;
                coolDownTime = cdTime; 
            }
        }

        public abstract void ExecuteAction(DarknessMinion controller);

        protected void RequestActionCooldown(DarknessMinion controller, float coolDownTime, ActionCooldownType actionCooldownType)
        {
            controller.ProcessActionCooldown(actionCooldownType, coolDownTime);
        }
        
        [SerializeField]
        private ActionFlags[] Conditions;

        private Dictionary<ActionFlags,Func<DarknessMinion,bool>> ActionFlagCheck = new Dictionary<ActionFlags, Func<DarknessMinion, bool>>
        {
            {ActionFlags.PlayerInAttackRange, PlayerInAttackRange},
            {ActionFlags.NavTargetDistClose, NavTargetDistCloseCheck},
            {ActionFlags.AttackOnCooldown, AttackOnCooldownCheck},
            {ActionFlags.IdleOnCooldown, IdleOnCooldownCheck},
            {ActionFlags.MovementOnCooldown, MovementOnCooldownCheck},
            {ActionFlags.EndOfPath, EndOfPathCheck},
            {ActionFlags.PlayerOutOfRange, PlayerOutOfRange},
            {ActionFlags.IdleOffCooldown, IdleOffCooldownCheck}
        };

        public bool ConditionsMet(DarknessMinion controller)
        {
            foreach(ActionFlags actCond in Conditions)
            {
                if(!CheckFlag(actCond, controller))
                    return false;
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

        private static bool IdleOnCooldownCheck(DarknessMinion controller)
        {
            return controller.idleOnCooldown;
        }

        private static bool IdleOffCooldownCheck(DarknessMinion controller)
        {
            if(!controller.idleOnCooldown)
                return true;
            else return false;
        }

        private static bool MovementOnCooldownCheck(DarknessMinion controller)
        {
            return controller.movementOnCooldown;
        }

        private static bool PlayerInAttackRange(DarknessMinion controller) 
        {
            if(controller.playerDist <= controller.switchTargetDistance) 
            {
                return true;
            }
            else return false;
        }

        private static bool AttackOnCooldownCheck(DarknessMinion controller) 
        {
            return controller.attackOnCooldown;
        }

        private static bool AttackOffCooldownCheck(DarknessMinion controller)
        {
            if(!controller.attackOnCooldown)
                return true;
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