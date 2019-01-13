using UnityEngine;

[CreateAssetMenu (menuName = "AI/Decision/ChaseDecision")]
public class ChaseDecision : AI_Decision
{
    public override bool Decide(Darkness controller)
    {   
        return controller.canAttack;
        //return ApproachPlayer(controller);
    }

    /* public bool ApproachPlayer(Darkness controller)
    {
        if(AI_Manager.Instance.CanMove(controller.creationID))
        {
            //Debug.Log("<b><color=blue>Chase:</color></b> Darkness #" + controller.queueID + " request has been processed");
            //AI_Manager.OnAttackRequest(controller.queueID, controller.attackRequested);
            return true;
        }
        else return false;
    }*/
}