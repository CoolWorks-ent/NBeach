using UnityEngine;

namespace Darkness
{
    [CreateAssetMenu (menuName = "Darkness/Action/AttackAction")]
    public class AttackAction : Dark_Action
    {
        [SerializeField]
        private float attackInitiationRange; 

        public override void ExecuteAction(DarknessMinion controller)
        {
            Darkness_Manager.OnRequestNewTarget(controller.creationID, true);
            if(controller.playerDist > attackInitiationRange)
            {
                //TODO add utility function on the Dark_State level for rotating the darkness to face the player
                //If the rotation is within a certain range say +/-5 degrees then try a sphere cast to see if I can attack
                //if that returns the player chomp em
            }
        }

        public override void TimedTransition(DarknessMinion controller)
        {
            
        }
    }
}