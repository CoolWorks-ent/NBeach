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

    [Range(1, 15)]
    public float waitRange;

    protected override void Awake()
    {
        stateType = StateType.WANDER;
        base.Awake();
    }

    public override void InitializeState(Darkness controller)
    {
        controller.animeController.SetTrigger(controller.wanderHash);
        controller.aIRichPath.canMove = true;
        controller.aIRichPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        controller.aIRichPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        float range = Random.RandomRange(minWanderRange, maxWanderRange);
        Vector3 t = new Vector3(range, 0, range);
        //controller.aIDestSet.target.position = new Vector3(controller.target.position.x + range, controller.transform.position.y, controller.transform.position.z + Mathf.Abs(range)/2);
    }

    public override void UpdateState(Darkness controller)
    {
       if(Vector3.Distance(controller.transform.position, controller.target.position) < controller.attackInitiationRange+waitRange)
       {
           controller.aIRichPath.canMove = false;
       }
       CheckTransitions(controller);
    }

    protected override void ExitState(Darkness controller)
    {
        controller.aIDestSet.target = controller.target;
        controller.aIRichPath.canMove = false;
        controller.standBy = false;
    }
}