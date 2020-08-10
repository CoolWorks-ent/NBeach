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

        [SerializeField, Range(0, 10)]
        protected float executionTime, coolDownTime;

        public enum ActionFlags { NavTargetDistClose, EndOfPath, PlayerInAttackRange, PlayerOutOfRange} //AttackOnCooldown, AttackOffCooldown, MovementOnCooldown, IdleOnCooldown, IdleOffCooldown,
        //public enum ActionCooldownType {Attack, AttackActive, Idle, Movement}

        public struct ActionCooldownInfo 
        {
            //public ActionCooldownType acType;
            public string name;
            public float durationTime;
            public float coolDownTime;

            public ActionCooldownInfo(string nam, float durTime, float cdTime)
            {
                name = nam;
                durationTime = durTime;
                coolDownTime = cdTime; 
            }
        }

        public abstract void ExecuteAction(DarknessMinion controller);//make virtual and add time update for how this action has been executing

        protected void TimedActionActivation(DarknessMinion controller, string name, float durationTime, float coolDownTime)
        {
            controller.ProcessActionCooldown(name, durationTime, coolDownTime);
        }

        protected Vector3 RandomPoint(Vector3 center, float radiusLower, float radiusUpper)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * Mathf.Sqrt(UnityEngine.Random.Range(radiusLower, radiusUpper));
            return new Vector3(point.x + center.x, center.y, point.y + center.z);
        }
        
        [SerializeField]
        private ActionFlags[] Conditions;

        private Dictionary<ActionFlags,Func<DarknessMinion,bool>> ActionFlagCheck = new Dictionary<ActionFlags, Func<DarknessMinion, bool>>
        {
            {ActionFlags.PlayerInAttackRange, PlayerInAttackRange},
            {ActionFlags.NavTargetDistClose, NavTargetDistCloseCheck},
            {ActionFlags.EndOfPath, EndOfPathCheck},
            {ActionFlags.PlayerOutOfRange, PlayerOutOfRange},
            //{ActionFlags.AttackOnCooldown, AttackOnCooldownCheck},
            //{ActionFlags.IdleOnCooldown, IdleOnCooldownCheck},
            //{ActionFlags.MovementOnCooldown, MovementOnCooldownCheck},
            
            //{ActionFlags.IdleOffCooldown, IdleOffCooldownCheck}
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

        private static bool ThisActionOnCooldown(DarknessMinion controller)
        {
            //TODO check if the action is in the cooldown list on the DarknessMinion
            return false; //controller.CheckActionsOnCooldown(this.name); //TODO this bullshit needs to be fixed. someway there needs to be an identifier to check this nonsense
        }

        /*private static bool IdleOnCooldownCheck(DarknessMinion controller)
        {
            return controller.idleOnCooldown;
        }

        private static bool IdleOffCooldownCheck(DarknessMinion controller)
        {
            if(!controller.idleOnCooldown)
                return true;
            else return false;
        }*/

        /*private static bool MovementOnCooldownCheck(DarknessMinion controller)
        {
            return controller.movementOnCooldown;
        }*/

        private static bool PlayerInAttackRange(DarknessMinion controller) 
        {
            if(controller.playerDist <= controller.switchTargetDistance) 
            {
                return true;
            }
            else return false;
        }

        /*private static bool AttackOnCooldownCheck(DarknessMinion controller) 
        {
            return controller.attackOnCooldown;
        }

        private static bool AttackOffCooldownCheck(DarknessMinion controller)
        {
            if(!controller.attackOnCooldown)
                return true;
            else return false;
        }*/

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