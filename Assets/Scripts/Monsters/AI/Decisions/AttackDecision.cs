using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/AttackDecision")]
public class AttackDecision : AI_Decision
{   
    public override bool Decide(Darkness controller)
    {
        return(controller.TargetWithinDistance());
    }

    /// <summary>
    /// Check to see if the darkness is within distance to attack the controller.
    /// </summary>
    
}