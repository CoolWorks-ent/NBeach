using UnityEngine;

namespace Darkness
{    
    public class PatrolAction : Dark_Action
    {
        public void OnEnable()
        {
            actionType = ActionType.Patrol;
        }

        public override void ExecuteAction(DarknessMinion controller)
        {
            //Check if the Darkness has finished moving then assign it a new location
            // if(controller.aIMovement.aI.remainingDistance <= 3)
            // {
            //     ChooseNewPatrolPoints(controller);
            //     controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
            // }
            //controller.aIMovement.UpdatePath(controller.Target.position);
            controller.UpdateAnimator(controller.chaseHash);
            if(controller.reachedEndOfPath)
            {
                controller.AddCooldown(new ActionCooldownInfo(coolDownTime, actionType));
                controller.navTarget.UpdateLocation(RandomPoint(controller.transform.position, 5, 10));
                controller.UpdatePath();
            }
        }

        protected Vector3 RandomPoint(Vector3 center, float radiusLower, float radiusUpper)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * Mathf.Sqrt(UnityEngine.Random.Range(radiusLower, radiusUpper));
            return new Vector3(point.x + center.x, center.y, point.y + center.z);
        }
    }
}