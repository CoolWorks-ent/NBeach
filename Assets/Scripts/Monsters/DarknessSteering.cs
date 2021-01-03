using UnityEngine;

namespace DarknessMinion
{
    public class DarknessSteering 
    {

        private Vector3[] SeekMap, AvoidMap;
        public Vector3[] CombinedMap { get; private set; }

        public Vector3 Seek(Vector3 targetPosition, DarknessMovement dMovement)
        {
            Vector3 desiredVelocity = (targetPosition - dMovement.position).normalized;

            return desiredVelocity - dMovement.velocity;
        }

        public void Flee(Vector3 fleeTarget)
        {

        }
    }
}