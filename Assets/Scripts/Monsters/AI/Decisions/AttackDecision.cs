using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/Decision/AttackDecision")]
public class AttackDecision : AI_Decision
{

    public override bool Decide(Darkness controller)
    {
        return(TargetWithinDistance(controller));
    }

    public bool TargetWithinDistance(Darkness controller)
    {
        if(Vector3.Distance(controller.target.position, controller.transform.position) <= controller.attackInitiationRange)
        {
            return true;
        }
        else return false;
    }
}