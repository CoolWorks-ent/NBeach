using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/Movement")]
    public class MoveTowardsAction : Dark_Action
    {
        public DarknessMinion.NavTargetTag navTargetType;

        public override void ExecuteAction(DarknessMinion controller)
        {
            if(controller.navTarget.navTargetTag != navTargetType)
                Darkness_Manager.OnRequestNewTarget(controller.creationID, false);
            controller.UpdatePath();
            controller.moving = true;
        }

    }
}