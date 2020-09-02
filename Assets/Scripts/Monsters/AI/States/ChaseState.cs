using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{
    [Range(0.25f, 5)]
    public float pathUpdateRate;
    protected override void FirstTimeSetup()
    {
        stateType = StateType.CHASING;
    }

    public override void InitializeState(Darkness controller)
    {
        //base.InitializeState(controller);
        Darkness_Manager.OnRequestNewTarget(controller.creationID);
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.pather.destination = controller.navTarget.navPosition;
        controller.pather.pickNextWaypointDist += 0.5f;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
        controller.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));

        /*controller.aIMovement.CreatePath(controller.Target.position);
        controller.aIMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        controller.aIMovement.moving = true;*/
        //controller.aIRichPath.endReachedDistance = stopDist;
    }

    public override void UpdateState(Darkness controller)
    {
        //controller.aIMovement.UpdatePath(controller.Target.position);
        
        CheckTransitions(controller);
    }

    public override void MovementUpdate(Darkness controller)
    {

    }

    protected override void CooldownCallback(Darkness controller)
    {
        controller.pather.destination = controller.navTarget.navPosition;
        controller.AddCooldown(new CooldownInfo(pathUpdateRate, CooldownStatus.Moving, CooldownCallback));
    }

    public override void ExitState(Darkness controller)
    {
        //controller.aIMovement.EndMovement();
        controller.pather.canMove = false;
        controller.pather.canSearch = false;
        controller.pather.pickNextWaypointDist -= 0.5f;
        controller.pather.endReachedDistance -= 1.0f;
        controller.sekr.CancelCurrentPathRequest();
    }
}