using UnityEngine;

namespace DarknessMinion
{
    public class DarknessSteering 
    {

        private Vector3[] SeekMap, AvoidMap;
        public Vector3[] CombinedMap { get; private set; }

        /*public Vector3 Seek(Vector3 targetPosition, Transform start)
        {
            Vector3 desiredVelocity = (targetPosition - start.position).normalized;

            return desiredVelocity - ;
        }*/

        public void Flee(Vector3 fleeTarget)
        {

        }
    }
}