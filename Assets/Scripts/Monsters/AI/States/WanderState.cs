using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
public class WanderState : ChaseState
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
            //controller.UpdatePath();
        }
        /*if(controller.playerDist < 3 && controller.pather.rotationSpeed < 360)
            controller.pather.rotationSpeed = 400;*/
        CheckTransitions(controller);
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

    public override void ExitState(Darkness controller)
    {
        controller.EndMovement();
        //controller.sekr.CancelCurrentPathRequest();
    }
}