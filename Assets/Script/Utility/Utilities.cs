using Unity.VisualScripting;
using UnityEngine;

namespace BA.Utility
{
    public class Utilities
    {
        public static bool ObjectVisible(Object obj, Camera cam)
        {
            if(obj.GetType() == typeof(Transform) || obj.GetType() == typeof(GameObject)) 
            {
                Bounds bounds = obj.GetComponent<Renderer>().bounds;

                return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds);
            }
            else
            {
                return false;
            }
        }

        public static Rect CalcucateTextureScreenPos(Vector3 pos, float size)
        {
            return new Rect(pos.x - size / 2, Screen.height - pos.y - size / 2, size, size);
        }

        public static int BoolToInt(bool b)
        {
            return b ? 1 : 0;
        }
    }
}
