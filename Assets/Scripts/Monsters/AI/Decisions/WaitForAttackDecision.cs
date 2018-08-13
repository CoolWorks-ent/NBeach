using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/AttackDecision")]
public class WaitForAttackDecision : AI_Decision
{

    public override bool Decide(Darkness controller)
    {
        return(CheckForAttack(controller));
    }

    public bool CheckForAttack (Darkness controller)
    {
        if(controller.canAttack && controller.currentState.stateType == EnemyState.CHASING)
            return true;
        else return false;
    }
}