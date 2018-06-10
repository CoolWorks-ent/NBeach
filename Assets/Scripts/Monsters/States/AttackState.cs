using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/State/AttackState")]
public class AttackState : DarkState
{
    public override void OnEnable()
    {
        stateType = EnemyState.ATTACK;
    }
    public override void InitializeState(Darkness owner, DarkStateController controller)
    {
        dsStateController = controller;
        owner.aIPath.canMove = false;
        Debug.Log("Entered attack state");
    }

    public override void UpdateState(Darkness owner)
    {
        
    }

    public override void ExitState(Darkness owner)
    {
        Debug.Log("Exiting attack state");
    }
}