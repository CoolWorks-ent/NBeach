using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/AttackDecision")]
public class AttackDecision : AI_Decision
{   
    public override bool Decide(Darkness controller)
    {
        return(CanAttack(controller));
    }

    /// <summary>
    /// Check to see if the darkness is within distance to attack the controller.
    /// </summary>
    private bool CanAttack(Darkness controller)
    {
        if(controller.TargetWithinDistance()) //&& AI_Manager.Instance.CanMove(controller.creationID))
        {
            return true;
        }
        else return false;
    }
}