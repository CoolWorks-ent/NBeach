using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AI/Actions/Attack")]
public class Attack_Action : AI_Action
{
    [Range(1, 3)]
    public int attackSpeedModifier;

    public override void Act(Darkness controller)
    {
        controller.aIRichPath.canMove = true;
        controller.animeController.SetTrigger(controller.attackHash);
    }
}