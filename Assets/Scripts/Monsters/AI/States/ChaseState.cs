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
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.aIMovement.CreatePath(controller.Target.position);
        controller.aIMovement.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIMovement.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        //controller.aIRichPath.endReachedDistance = stopDist;
    }

    public override void UpdateState(Darkness controller)
    {
        controller.aIMovement.UpdatePath(controller.Target.position);

        CheckTransitions(controller);
    }

    protected override void ExitState(Darkness controller)
    {
        controller.aIMovement.EndMovement();
    }
}