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
       /* controller.pather.destination = controller.Target.location.position;
        controller.pather.pickNextWaypointDist += 0.5f;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;*/
        controller.darkMovement.CreatePath(controller.Target.location.position);
        //controller.darkMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
        //controller.darkMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        controller.darkMovement.moving = true;
        //controller.aIRichPath.endReachedDistance = stopDist;
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.aIMovement.UpdatePath(controller.Target.position);
        //controller.pather.destination = controller.Target.location.position;
        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        //controller.aIMovement.EndMovement();
        /*controller.pather.canMove = false;
        controller.pather.canSearch = false;
        controller.pather.pickNextWaypointDist -= 0.5f;
        controller.pather.endReachedDistance -= 1.0f;*/
        controller.sekr.CancelCurrentPathRequest();
    }
}