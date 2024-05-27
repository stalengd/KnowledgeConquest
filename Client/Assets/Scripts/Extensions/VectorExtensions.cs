using UnityEngine;

namespace KnowledgeConquest.Client.Extensions
{
    public static class VectorExtensions 
    {
        public static Vector3 FromXZPlane(this Vector2 pos2d, float newY = 0f)
        {
            return new Vector3(pos2d.x, newY, pos2d.y);
        }

        public static Vector2 ToXZPlane(this Vector3 worldPos)
        {
            return new Vector2(worldPos.x, worldPos.z);
        }
    }
}
