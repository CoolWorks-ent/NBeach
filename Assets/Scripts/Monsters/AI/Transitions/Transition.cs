using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_Transition
{
    public AI_Decision decision;
    public AI_State trueState;
    public AI_State falseState;
}