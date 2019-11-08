using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
public class WanderState : ChaseState
{
    [Range(8,20)]
    public int relativeRangeToPlayer;
    [Range(1,8)]
    public int wanderRadius;
    protected override void FirstTimeSetup()
    {
        stateType = StateType.WANDER;
    }

    public override void Startup()
    {
        AI_Manager.UpdateMovement += UpdatePatrolPoints;
        base.Startup();
    }

    public override void InitializeState(Darkness controller)
    {
        base.InitializeState(controller);
        controller.pather.destination = controller.Target.position;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
        controller.pather.repathRate = 3.0f;
        controller.pather.pickNextWaypointDist = 0.95f;
        controller.pather.rotationSpeed = 180;
        //controller.aIMovement.wandering = true;
        //ChooseNewPatrolPoints(controller);
        /*controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
        AI_Manager.Instance.StartCoroutine(NewPointBuffer(controller,5));
        controller.aIMovement.CreatePath(controller.Target.position);
        controller.aIMovement.moving = true;
        controller.aIMovement.speed = 10;*/
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
        controller.pather.destination = controller.Target.position;
        if(controller.targetDist < 3 && controller.pather.rotationSpeed < 360)
            controller.pather.rotationSpeed = 400;
        CheckTransitions(controller);
    }

    private void UpdatePatrolPoints()
    {

    }

    /*private void ChooseNewPatrolPoints(Darkness controller)
    {
        Vector3 direction =  (controller.Target.transform.position - controller.transform.position).normalized;
        Vector3 wTemp = controller.aIMovement.wayPoint + direction*relativeRangeToPlayer;
        Vector3[] temp = new Vector3[controller.aIMovement.PatrolPoints.Length];

        for(int i = 0; i < temp.Length; i++)
        {
            Vector3 t = Random.insideUnitSphere * wanderRadius;
            controller.aIMovement.PatrolPoints[i] = new Vector3(t.x, controller.transform.position.y, t.z);
        }
        controller.aIMovement.PatrolPoints = temp;
    }

    private Vector3 ChoosePatrolPoint(Darkness controller)
    {
       return controller.aIMovement.PatrolPoints[Random.Range(0,controller.aIMovement.PatrolPoints.Length)];
    }

    protected IEnumerator NewPointBuffer(Darkness controller, float idleTime)
    {
        while(controller.aIMovement.wandering)
        {
            controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
            yield return new WaitForSeconds(idleTime);
        }
        yield return null;
    }*/

    protected override void ExitState(Darkness controller)
    {
        //controller.aIMovement.EndMovement();
        //controller.aIMovement.wandering = false;
        controller.pather.canMove = false;
        controller.pather.canSearch = false;
        controller.sekr.CancelCurrentPathRequest();
    }
}