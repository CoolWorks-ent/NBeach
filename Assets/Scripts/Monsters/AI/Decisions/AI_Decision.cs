using UnityEngine;

public abstract class AI_Decision : ScriptableObject
{
    public enum Decision_Priority {HIGH = 1, MEDIUM, LOW}
    public Decision_Priority priority;
    public abstract bool Decide(Darkness controller);
}