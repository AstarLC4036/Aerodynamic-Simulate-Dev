using BA.Keybinding;
using System.Collections;
using UnityEngine;

namespace BA.CameraUtil
{
    public class FixedCamera : MonoBehaviour
    {
        public Transform aircraft;

        public Pov[] povs;
        public Transform landPov;
        public float cameraMoveAlpha = 0.5f;
        public float mouseScaleMultiplyer = 10;
        public float rotateSpeedMulti = 2;

        private Camera cameraComponent;
        private int index = 0;
        private bool moveCam = false;

        private Vector3[] originPovPositions;
        private Quaternion[] originPovRotations;

        // Use this for initialization
        void Start()
        {
            cameraComponent = GetComponent<Camera>();
            originPovPositions = new Vector3[povs.Length];
            originPovRotations = new Quaternion[povs.Length];
            for (int i = 0; i < povs.Length; i++)
            {
                originPovPositions[i] = povs[i].transform.localPosition;
                originPovRotations[i] = povs[i].transform.localRotation;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyBinding.cam1))
            {
                index = 0;
            }
            else if (Input.GetKeyDown(KeyBinding.cam2))
            {
                index = 1;
            }
            else if (Input.GetKeyDown(KeyBinding.cam3))
            {
                index = 2;
            }
            else if (Input.GetKeyDown(KeyBinding.camLand))
            {
                index = -1;
            }
        }

        private void FixedUpdate()
        {
            if (index != -1 && povs[index] != null)
            {
                //camera follow
                if (!povs[index].isFixedCamera)
                {
                    if (transform.parent != null && !povs[index].canRotate)
                        transform.SetParent(null);

                    float posAlpha = (povs[index].transform.position - transform.position).magnitude <= cameraMoveAlpha ? 1 : cameraMoveAlpha;
                    Vector3 nextPos = Vector3.Lerp(transform.position, povs[index].transform.position, posAlpha);
                    transform.position = nextPos;

                    float rotationAlpha = Mathf.Abs((povs[index].transform.eulerAngles - transform.eulerAngles).magnitude) <= cameraMoveAlpha ? 1 : cameraMoveAlpha;
                    Quaternion nextRotation = Quaternion.Lerp(transform.rotation, povs[index].transform.rotation, rotationAlpha);
                    transform.rotation = nextRotation;

                    //float camMoveSpeed = ((transform.position - povs[index].transform.position) / (Time.deltaTime * cameraMoveTime)).magnitude;
                    //transform.position = Vector3.MoveTowards(transform.position, povs[index].transform.position, Time.deltaTime * camMoveSpeed);
                    //float camRotateSpeed = ((transform.eulerAngles - povs[index].transform.eulerAngles) / (Time.deltaTime * cameraMoveTime)).magnitude;
                    //transform.rotation = Quaternion.Euler(Vector3.MoveTowards(transform.eulerAngles, povs[index].transform.eulerAngles, Time.deltaTime * camRotateSpeed));
                    //transform.forward = povs[index].forward;
                }
                else if (povs[index].isFixedCamera)
                {
                    if(transform.parent != aircraft)
                        transform.SetParent(aircraft);
                    if (transform.position != povs[index].transform.position || transform.rotation != povs[index].transform.rotation)
                    {
                        transform.position = povs[index].transform.position;
                        transform.rotation = povs[index].transform.rotation;
                    }
                }

                //camera move
                if (povs[index].canRotate)
                {
                    if (transform.parent != aircraft)
                    {
                        transform.SetParent(aircraft);
                        transform.position = povs[index].transform.position;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        moveCam = true;
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        moveCam = false;
                        povs[index].transform.localPosition = originPovPositions[index];
                        povs[index].transform.localRotation = originPovRotations[index];
                    }

                    if (moveCam)
                    {
                        float mouseX = Input.GetAxis("Mouse X");
                        float mouseY = Input.GetAxis("Mouse Y");
                        Vector3 rotateCenter = povs[index].useRotateCenter ? povs[index].rotateCenter + povs[index].transform.position : aircraft.position;
                        povs[index].transform.RotateAround(rotateCenter, transform.up, mouseX * rotateSpeedMulti * Time.deltaTime);
                        povs[index].transform.RotateAround(rotateCenter, transform.right, mouseY * rotateSpeedMulti * Time.deltaTime);
                        povs[index].transform.LookAt(rotateCenter);
                    }
                }
            }
            else if(index == -1)
            {
                transform.position = landPov.position;
                transform.rotation = landPov.rotation;
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                cameraComponent.fieldOfView -= Input.mouseScrollDelta.y * mouseScaleMultiplyer;
            }
        }
    }
}