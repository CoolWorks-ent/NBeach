using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{

    [Range(0, 15)]
    public float stopDist;
    [Range(1,8)]
    public float minSpeedRange, maxSpeedRange;
    [Range(0,5)]
    public float minRepathRate, maxRepathRate;

    protected override void FirstTimeSetup()
    {
        stateType = StateType.CHASING;
    }

    public override void InitializeState(Darkness controller)
    {
        RequestNewTarget(controller.creationID);
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.pather.destination = controller.Target.location.position;
        controller.pather.pickNextWaypointDist = 0.2f;
        controller.pather.repathRate = 2.25f;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
        controller.pather.rotationSpeed = 180;
        /*controller.aIMovement.CreatePath(controller.Target.position);
        controller.aIMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        controller.aIMovement.moving = true;*/
        //controller.aIRichPath.endReachedDistance = stopDist;
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.aIMovement.UpdatePath(controller.Target.position);
        controller.pather.destination = controller.Target.location.position;
        if(controller.playerDist < 3 && controller.pather.rotationSpeed < 360)
            controller.pather.rotationSpeed = 400;
        CheckTransitions(controller);
    }

    protected override void ExitState(Darkness controller)
    {
        //controller.aIMovement.EndMovement();
        controller.pather.canMove = false;
        controller.pather.canSearch = false;
        controller.sekr.CancelCurrentPathRequest();
    }
}