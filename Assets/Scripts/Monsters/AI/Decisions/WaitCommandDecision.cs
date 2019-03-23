using System;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/WaitDecision")]
public class WaitCommandDecision : AI_Decision
{
    public override bool Decide(Darkness controller)
    {
        if(controller.aIRichPath.canMove) 
        {
            return true;
        }
        else return false;
    }
}