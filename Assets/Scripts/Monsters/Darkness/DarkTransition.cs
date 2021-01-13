namespace DarknessMinion
{ 
    [System.Serializable]
    public class DarkTransition
    {
        public enum TransitionPriority {High, Medium, Low}
        public DarkDecisionMaker decision = new DarkDecisionMaker();
        public DarkDecisionMaker.DecisionName decisionChoice;
        public TransitionPriority priorityLevel;
        public DarkState trueState;
        
    }
}