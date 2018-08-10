using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/Darkness/State/DeathState")]
public class DeathState : AI_State
{
    public void InitializeState(Darkness controller)
    {
        controller.aIRichPath.canMove = false;
        controller.animeController.SetTrigger(controller.deathHash);
        
        ExitState(controller);
    }

    public void UpdateState(Darkness controller)
    {
    }

    public void ExitState(Darkness controller)
    {
        AI_Manager.OnDarknessRemoved(controller.queueID);
    }

}
