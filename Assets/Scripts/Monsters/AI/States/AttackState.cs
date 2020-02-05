using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "AI/Darkness/State/AttackState")]
public class AttackState : Dark_State
{
    [Range(1, 3)]
    public int attackSpeedModifier;
    [Range(0,5)]
    public float attackCooldown;

    protected override void FirstTimeSetup()
    {
        stateType = StateType.ATTACK;
    }

    public override void InitializeState(Darkness controller)
    {
        AI_Manager.OnRequestNewTarget(controller.creationID);
        controller.pather.destination = controller.Target.location.position;
        controller.pather.repathRate = 1.75f;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
        controller.pather.rotationSpeed = 270;
        controller.pather.endReachedDistance = 1.85f;
        //controller.aIRichPath.maxSpeed *= attackSpeedModifier;
        controller.pather.canMove = true;
        controller.pather.canSearch = true;
    }

    public override void UpdateState(Darkness controller)
    { 
        //TODO check if the darkness is facing the player. if not start rotating towards the player
        controller.pather.destination = controller.Target.location.position;
        if(controller.playerDist < controller.attackInitiationRange && !controller.attacked) 
        {
            controller.darkHitBox.enabled = true;
            controller.pather.canMove = false;
            controller.animeController.SetTrigger(controller.attackHash);
            controller.attacked = true;
            //if(controller.animeController.animation.)
            //controller.StartCoroutine(controller.AttackCooldown(attackCooldown, controller.idleHash));
        }   
        /*else 
        {
            controller.pather.canMove = true;
        }
        CheckTransitions(controller);*/
    }

    public override void ExitState(Darkness controller)
    {
        controller.pather.endReachedDistance = 0.2f;
        controller.attacked = false;
        controller.darkHitBox.enabled = false;
        controller.animeController.SetTrigger(controller.idleHash);
        //controller.animeController.SetBool(controller.attackAfterHash, true);
    }
}