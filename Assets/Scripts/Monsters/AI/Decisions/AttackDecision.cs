using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/AttackDecision")]
public class AttackDecision : AI_Decision
{   
    public override bool Decide(Darkness controller)
    {
        return(TargetWithinDistance(controller));
    }

    public bool TargetWithinDistance(Darkness controller)
    {
        if(Vector3.Distance(controller.target.position, controller.transform.position) <= controller.attackInitiationRange && controller.canAttack)
        {
            controller.canAttack = false;
            return true;
        }
        else return false;
    }
}