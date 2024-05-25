using UnityEngine;
using KnowledgeConquest.Shared.Math;

namespace KnowledgeConquest.Client.Extensions
{
    public static class OffsetCoordsExtensions
    {
        public static Vector2Int AsVector2Int(this OffsetCoords offsetCoords)
        {
            return new Vector2Int(offsetCoords.X, offsetCoords.Y);
        } 

        public static OffsetCoords AsOffsetCoords(this Vector2Int vector)
        {
            return new OffsetCoords(vector.x, vector.y);
        } 
    }
}
