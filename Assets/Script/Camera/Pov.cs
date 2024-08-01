using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BA.CameraUtil
{
    public class Pov : MonoBehaviour
    {
        public bool isFixedCamera = false;
        public bool canRotate = false;
        public bool useRotateCenter = false;
        public Vector3 rotateCenter = Vector3.zero;
        [Tooltip("If this is true, when user scroll the scroller of mouse, it will adjust camera's zoom,\nIf this is false, when user scroll the scroller of the mouse, it will change the distance to the camera's focus center.")]
        public bool isZoomMode = true;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position + rotateCenter, 1);        
        }
    }
}
