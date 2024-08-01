using System.Collections;
using UnityEngine;

namespace BA.CameraUtil
{
    public class CameraFocus : MonoBehaviour
    {
        public Transform parent;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(parent.position);
        }
    }
}