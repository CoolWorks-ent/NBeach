using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/State/ChaseState")]
public class ChaseState : DarkState
{

    [Range(0.5f,12.0f)]
    public float minRepathRate, maxRepathRate;
    [Range(1.0f,10.0f)]
    public float minSpeedRange, maxSpeedRange;
    
    public override void OnEnable()
    {
        stateType = EnemyState.CHASING;
    }

    public override void InitializeState(Darkness owner, DarkStateController controller)
    {
        dsStateController = controller;   
        owner.aIPath.canMove = true;
        owner.aIDestSetter.target = owner.target;
        owner.aIPath.repathRate = Random.Range(minRepathRate, maxRepathRate);
        owner.aIPath.maxSpeed = Random.Range(minSpeedRange, maxSpeedRange);
        Debug.Log("Entering chase sequence");
    }

    public override void UpdateState(Darkness owner)
    {
        
    }

    public override void ExitState(Darkness owner)
    {
        owner.aIPath.canMove = false;
        Debug.Log("Exiting chase sequence");
    }
}