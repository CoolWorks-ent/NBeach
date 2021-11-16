namespace Darkness.States
{ 
    [System.Serializable]
    public class Transition
    {
        public enum TransitionPriority {High, Medium, Low}
        public DecisionMaker decision = new DecisionMaker();
        public DecisionMaker.DecisionName decisionChoice;
        public TransitionPriority priorityLevel;
        public DarkState trueState;
        
    }
}