using UnityEngine;

namespace DarknessMinion.Movement
{
    public class DirectionNode
    {
        public float angle { get; private set; }
        public float degAngle { get; private set; }
        public float avoidWeight, seekWeight;
        public float combinedWeight { get { return avoidWeight + seekWeight; } }

        public float dirLength { get; private set; }

        public Vector2 directionAtAngle { get; private set; }

        public DirectionNode(float rAngle, float dAngle)
        {
            angle = rAngle;
            degAngle = dAngle;
            avoidWeight = 0;
            seekWeight = 0;
            directionAtAngle = Vector2.zero;
            dirLength = 0;
        }

        public void SetDirection(Vector2 v)
        { 
            directionAtAngle = v; 
        }

        public void SetDirLength(float v)
        {
            dirLength = v;
        }
    }
}