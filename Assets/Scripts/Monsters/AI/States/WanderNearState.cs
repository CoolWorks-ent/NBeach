using UnityEngine;

[CreateAssetMenu (menuName = "AI/Darkness/State/WanderNearState")]
public class WanderNearState : Dark_State
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;

    [Range(-14.0f, 14.0f)]
    public float minWanderRange, maxWanderRange;

    protected override void Awake()
    {
        stateType = StateType.WANDER;
        base.Awake();
    }

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.chaseHash);
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        float range = Random.RandomRange(minWanderRange, maxWanderRange);
        controller.aIDestSet.target.position = new Vector3(controller.target.position.x + range, controller.transform.position.y, controller.transform.position.z + Mathf.Abs(range)/2);
    }

    public override void UpdateState(Darkness controller)
    {
       CheckTransitions(controller);
    }

    protected override void ExitState(Darkness controller)
    {
        controller.aIDestSet.target = controller.target;
        controller.aIRichPath.canMove = false;
    }
}