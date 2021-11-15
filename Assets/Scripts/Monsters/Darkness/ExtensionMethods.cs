using UnityEngine;

namespace DarknessMinion
{
    public static class ExtensionMethods
    {
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 ToVector3(this Vector2 vec, float height = 0)
        {
            return new Vector3(vec.x, height, vec.y);
        }
    }
}