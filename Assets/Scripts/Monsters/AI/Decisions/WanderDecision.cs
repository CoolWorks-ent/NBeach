using System;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/WanderDecision")]
public class WanderDecision : AI_Decision
{
    public override bool Decide(Darkness controller)
    {
        if(controller.standBy && Vector3.Distance(controller.target.position,controller.transform.position) > controller.waitRange) //&& Vector3.Distance(controller.transform.position,controller.target.position) < controller.waitRange)
        {
            return true;
        }
        else return false;
    }
}
