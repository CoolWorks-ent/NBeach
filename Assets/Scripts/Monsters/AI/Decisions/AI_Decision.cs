using UnityEngine;

public abstract class AI_Decision : ScriptableObject
{
    public abstract bool Decide(Darkness controller);
    public string description;
}