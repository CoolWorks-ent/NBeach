using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Darkness/State/DeathState")]
public class DeathState : DarkState
{
	    public override void OnEnable()
    {
        stateType = EnemyState.DEATH;
    }
    public override void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.deathHash);
        
        ExitState(controller);
    }

    public override void UpdateState(Darkness controller)
    {
    }

    public override void ExitState(Darkness controller)
    {
        AI_Manager.OnDarknessRemoved(controller.queueID);
    }

}
