using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{

    protected override void FirstTimeSetup()
    {
        stateType = StateType.CHASING;
    }

    public override void InitializeState(Darkness controller)
    {
        base.InitializeState(controller);
        AI_Manager.OnRequestNewTarget(controller.creationID);
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.StartCoroutine(controller.UpdatePath());
        controller.moving = true;
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.UpdatePath();
        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        controller.EndMovement();
    }
}