using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
public class WanderState : Dark_State
{
    //[Range(8,20)]
    //public int relativeRangeToPlayer;
    [Range(1,10)]
    public int chooseNewTargetDist;
    [Range(1,10)]
    public float patrolDistLow, patrolDistUpper;

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
        //controller.patrolNavTarget.active = true;
        //controller.navTarget.UpdateLocation(ChoosePatrolPoint(controller));
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.navTarget.UpdateLocation(RandomPoint(controller.transform.position, patrolDistLow, patrolDistUpper));
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
        if(controller.targetDistance <= controller.swtichDist/2)
        {
            controller.navTarget.UpdateLocation(RandomPoint(controller.transform.position, 5, 10));
            controller.UpdatePath();
        }

        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        controller.EndMovement();
        //controller.sekr.CancelCurrentPathRequest();
    }

    /*private Vector3 ChoosePatrolPoint(Darkness controller)
    {
        Vector3 direction = Random.onUnitSphere * wanderRadius;
        direction.y = controller.transform.position.y;
        
        return direction;
    }*/
}