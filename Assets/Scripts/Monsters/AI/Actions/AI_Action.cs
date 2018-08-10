using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_Action : ScriptableObject
{
    public abstract void Act(Darkness controller);
}