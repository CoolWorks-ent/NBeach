using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/Movement")]
    public class MoveTowardsAction : Dark_Action
    {
        public void OnEnable()
        {
            actionType = ActionType.Chase;
        }

        public override void ExecuteAction(DarknessMinion controller)
        {
            //Add a bool to DarknessMinion for checking if the proper navTargets are established
            Darkness_Manager.OnRequestNewTarget(controller.creationID);
            controller.UpdateAnimator(controller.chaseHash);
            controller.UpdatePath();
            controller.ResumeMovement();
        }
    }
}