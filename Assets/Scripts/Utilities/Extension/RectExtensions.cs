using UnityEngine;

namespace Utilities.Extension
{
    public static class RectExtensions
    {
        public static Vector3 GetPointInRect(this Rect rect, in float yPos, in float margin)
        {
            var xPos = Random.Range(rect.xMin + margin, rect.xMax - margin);
            var zPos = Random.Range(rect.yMin + margin, rect.yMax - margin);

            return new Vector3(xPos, yPos, zPos);
        }
    }
}