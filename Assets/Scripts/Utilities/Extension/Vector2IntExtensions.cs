using UnityEngine;

namespace Utilities.Extension
{
    public static class Vector2IntExtensions
    {
        public static int GetRandomRange(this Vector2Int vector2Int, in bool inclusive = false)
        {
            if (vector2Int.x == vector2Int.y)
                return vector2Int.x;
            
            
            return inclusive
                ? Random.Range(vector2Int.x, vector2Int.y + 1)
                : Random.Range(vector2Int.x, vector2Int.y + 1);
        }
    }
}