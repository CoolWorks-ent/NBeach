using System.Collections.Generic;
using UnityEngine;

namespace DarknessMinion
{
    public class Avoidable 
    {
        public readonly Transform transform;
        private Bounds bounds;
        public Vector2 hitPosition;
        public readonly int objHashCode;
        public readonly AvoidableComparer comparer;

        public Avoidable(Transform t, Bounds b, Vector2 initialHitPosition, int gameObjectHashCode)
        {
            transform = t;
            bounds = b;
            hitPosition = initialHitPosition;
            objHashCode = gameObjectHashCode;
            comparer = new AvoidableComparer();
        }

        public Vector2 PositionVector2()
        {
            return transform.position.ToVector2();
        }

        public void UpdateHitPosition(Vector2 v)
        {
            hitPosition = v;
        }

        public Bounds GetBounds()
        {
            bounds.center = transform.position;
            return bounds;
        }

        public float Distance(Vector2 start)
        {
            return Vector2.Distance(transform.position.ToVector2(), start);
        }

        public Vector2 Direction(Vector2 start)
        {
            return transform.position.ToVector2() - start;
        }
    }
    
    public class AvoidableComparer : IEqualityComparer<Avoidable>
    {
        public bool Equals(Avoidable a1, Avoidable a2)
        {
            if(a1 != null && a2 != null)
                return a1.objHashCode == a2.objHashCode;
            return false;
        }

        public int GetHashCode(Avoidable obj)
        {
            return obj.objHashCode;
        }
    }
}