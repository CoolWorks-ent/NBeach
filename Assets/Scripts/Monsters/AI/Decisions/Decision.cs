using UnityEngine;

public abstract class Decision<T_Controller> : ScriptableObject
{
    public abstract bool Decide(T_Controller controller);
}