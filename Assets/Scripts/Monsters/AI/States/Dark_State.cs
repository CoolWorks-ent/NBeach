using UnityEngine;
using System.Collections;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/State/DarkState")]
    public class Dark_State : ScriptableObject
    {
        public enum SpecialStateType {NONE, DEATH, REMAIN}

        public SpecialStateType specialStateType;
        public Dark_Transition[] transitions;
        public Dark_Action[] actions;

        //[SerializeField, Range(0, 15)]
        //protected float stopDist;

        //[SerializeField, Range(0,360)]
        //protected int rotationSpeed;

        //[SerializeField, Range(0, 5)]
        //protected float pathUpdateRate;

        //[SerializeField]
        //protected bool hasExitTimer;

        //[Range(0, 5)]
        //public float exitTime;

        public virtual void InitializeState(DarknessMinion controller)
        {
            /*controller.pather.rotationSpeed = rotationSpeed;
            controller.pather.endReachedDistance = stopDist;
            controller.pather.maxSpeed = speedModifier;
            controller.pather.repathRate = pathUpdateRate;*/
            //if(hasExitTimer)
                //controller.StartCoroutine(controller.WaitTimer(exitTime));
        }
        public void UpdateState(DarknessMinion controller)
        {
            //if(!hasExitTimer)
            ExecuteActions(controller);
            CheckTransitions(controller);
        }

        protected void ExecuteActions(DarknessMinion controller) //check if action has proper flags checked for 
        {
            //Debug.Break();
            foreach(Dark_Action d_Action in actions)
            {
                //d_Action.ExecuteAction(controller);
                if(d_Action.ConditionsMet(controller))
                {
                    d_Action.ExecuteAction(controller);
                }
            }
        }

        protected void CheckTransitions(DarknessMinion controller)
        {
            for(int i = 0; i < transitions.Length; i++)
            {
                bool decisionResult = transitions[i].decision.MakeDecision(transitions[i].decisionChoice,controller);
                if(decisionResult) 
                {
                    if(transitions[i].trueState.specialStateType == SpecialStateType.REMAIN)
                        continue;
                    else controller.ChangeState(transitions[i].trueState); 
                }
                else if(!decisionResult) 
                {
                    if(transitions[i].falseState.specialStateType == SpecialStateType.REMAIN)
                        continue;
                    else controller.ChangeState(transitions[i].falseState);
                }
            }   
        }

        protected void RemoveDarkness(DarknessMinion controller)
        {
            if(specialStateType != Dark_State.SpecialStateType.DEATH)
            {
                //this.ExitState(controller);
                //controller.updateStates = false;
                //controller.ChangeState(controller.DeathState);
            }
        }
    }
}