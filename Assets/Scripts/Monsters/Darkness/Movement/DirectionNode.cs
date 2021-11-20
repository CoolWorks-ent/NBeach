using UnityEngine;

namespace Darkness.Movement
{
    public class DirectionNode
    {
        public float angle { get; private set; }
        public float degAngle { get; private set; }
        public float avoidWeight, seekWeight;
        public float combinedWeight { get { return avoidWeight + seekWeight; } }

        public float dirLength { get; private set; }

        public Vector2 directionAtAngle { get; private set; }

        public DirectionNode(float radianAngle)
        {
            angle = radianAngle;
            degAngle = angle * Mathf.Rad2Deg;
            avoidWeight = 0;
            seekWeight = 0;
            directionAtAngle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            dirLength = 0;
        }

        public void SetDirLength(float v)
        {
            dirLength = v;
        }

        public void UpdateDirection(Vector2 transformForward)
        {
            directionAtAngle = (directionAtAngle + transformForward).normalized;
        }
    }
}