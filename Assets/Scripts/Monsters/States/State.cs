using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T_Controller> : ScriptableObject
{
    public virtual void OnEnable(T_Controller controller) { }
    public virtual void InitializeState(T_Controller controller) { }
    public abstract void UpdateState(T_Controller controller);
    public abstract void ExitState(T_Controller controller);
}
