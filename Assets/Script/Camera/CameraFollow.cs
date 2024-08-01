using System.Collections;
using System.Threading;
using UnityEngine;

namespace BA.CameraUtil
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform parent;
        public Vector3 offset = new Vector3(0, 0, 0);
        public float focusOffset = 0;
        public float moveSpeedMultiplier = 3;

        private bool moveCam = false;

        // Use this for initialization
        void Start()
        {
            transform.localPosition = offset;
            transform.LookAt(parent);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                moveCam = true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                moveCam = false;
            }

            if(moveCam)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                offset = RotateRound(offset, Vector3.zero, Vector3.up, mouseX * moveSpeedMultiplier);
                offset = RotateRound(offset, Vector3.zero, Vector3.forward, mouseY * moveSpeedMultiplier);
            }

            transform.localPosition = offset;

            transform.LookAt(parent.position + focusOffset * parent.right, parent.transform.up);
        }

        public Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * (position - center) + center;
        }
    }
}