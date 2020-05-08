using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
public class WanderState : Dark_State
{
    //[Range(8,20)]
    //public int relativeRangeToPlayer;
    [Range(4,20)]
    public int wanderRadius;
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
        controller.patrolNavTarget.active = true;
        controller.patrolNavTarget.UpdateLocation(ChoosePatrolPoint(controller), false);
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.UpdatePath();
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
        if(controller.targetDistance <= controller.swtichDist)
        {
            controller.patrolNavTarget.UpdateLocation(ChoosePatrolPoint(controller), false);
        }

        CheckTransitions(controller);
    }

    private Vector3 ChoosePatrolPoint(Darkness controller)
    {
        Vector3 direction = Random.onUnitSphere * wanderRadius;
        direction.y = controller.transform.position.y;
        
        return direction;
    }

    public override void ExitState(Darkness controller)
    {
        controller.EndMovement();
        //controller.sekr.CancelCurrentPathRequest();
    }
}