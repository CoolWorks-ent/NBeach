namespace Darkness
{    
    public class PatrolAction : Dark_Action
    {
        public override void ExecuteAction(DarknessMinion controller)
        {
            //Check if the Darkness has finished moving then assign it a new location
            // if(controller.aIMovement.aI.remainingDistance <= 3)
            // {
            //     ChooseNewPatrolPoints(controller);
            //     controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
            // }
            //controller.aIMovement.UpdatePath(controller.Target.position);
            if(controller.reachedEndOfPath)
            {
                controller.navTarget.UpdateLocation(RandomPoint(controller.transform.position, 5, 10));
                controller.UpdatePath();
            }
        }

    }
}