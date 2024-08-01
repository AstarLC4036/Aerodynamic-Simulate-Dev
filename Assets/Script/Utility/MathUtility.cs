using UnityEngine;

namespace BA.Utility
{
    public class MathUtility
    {
        /// <summary>
        /// Floor to custom decimal places
        /// </summary>
        public static float FloorToDC(float num, int decimalPlaces)
        {
            return Mathf.FloorToInt(num * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        }

        public static float TransformAngle(float angle, float fov, float pixelHeight)
        {
            return (Mathf.Tan(angle * Mathf.Deg2Rad) / Mathf.Tan(fov / 2 * Mathf.Deg2Rad)) * pixelHeight / 2;
        }

        public static float CovertAngle(float angle)
        {
            if(angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }

        public static float CovertAngle360(float angle)
        {
            if(angle < 0)
            {
                angle = 360 + angle;
            }

            return angle;
        }
    }
}
