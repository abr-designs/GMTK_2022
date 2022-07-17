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
                : Random.Range(vector2Int.x, vector2Int.y);
        }

    }

    public static class Vector2Extensions
    {
        public static float GetRandomRange(this Vector2 vector2)
        {
            return vector2.x == vector2.y ? vector2.x : Random.Range(vector2.x, vector2.y);
        }
    }

}