using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
public class WanderState : Dark_State
{
    //[Range(8,20)]
    //public int relativeRangeToPlayer;
    //[Range(1,8)]
    //public int wanderRadius;
    protected override void FirstTimeSetup()
    {
        stateType = StateType.WANDER;
    }

    public override void Startup()
    {
        base.Startup();
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
        //Check if the Darkness has finished moving then assign it a new location
        // if(controller.aIMovement.aI.remainingDistance <= 3)
        // {
        //     ChooseNewPatrolPoints(controller);
        //     controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
        // }
        //controller.aIMovement.UpdatePath(controller.Target.position);
        if(controller.navTargetDist <= controller.swtichDist)
        {
            AI_Manager.OnRequestNewTarget(controller.creationID);
        }

        CheckTransitions(controller);
    }

    private Vector3 ChoosePatrolPoint(Darkness controller)
    {
       return new Vector3();
    }

    public override void ExitState(Darkness controller)
    {
        controller.EndMovement();
        //controller.sekr.CancelCurrentPathRequest();
    }
}