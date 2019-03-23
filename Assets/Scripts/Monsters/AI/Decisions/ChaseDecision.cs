using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/ChaseDecision")]
public class ChaseDecision : AI_Decision
{
    public override bool Decide(Darkness controller)
    {   
        //return controller.canAttack;
        return ApproachPlayer(controller);
    }

    public bool ApproachPlayer(Darkness controller)
    {
        if(controller.canAttack)
            return true;
        else return false;
    }
}