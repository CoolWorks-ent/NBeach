using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/ChaseDecision")]
public class ChaseDecision : AI_Decision
{
    public override bool Decide(Darkness controller)
    {   
        return ApproachTarget(controller);
    }

    public bool ApproachTarget(Darkness controller)
    {
        if(!controller.TargetWithinDistance())//Vector3.Distance(controller.target.position, controller.transform.position) >= controller.attackInitiationRange)
        {
            return true; //keep chasing if not in initiation range
        }
        else 
        {
            Debug.Log("<b><color=blue>Chase:</color></b> Darkness #" + controller.queueID + " request has been processed");
            AI_Manager.OnAttackRequest(controller.queueID, controller.attackRequested);
            return false;
        }
        
    }
}