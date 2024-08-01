using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BA.Keybinding
{
    public class KeyBinding : MonoBehaviour
    {
        public static KeyCode ignite = KeyCode.Space;
        public static KeyCode rotateZLeft = KeyCode.Q;
        public static KeyCode rotateZRight = KeyCode.E;
        public static KeyCode rotateYLeft = KeyCode.A;
        public static KeyCode rotateYRight = KeyCode.D;
        public static KeyCode rotateDown = KeyCode.W;
        public static KeyCode rotateUp = KeyCode.S;

        public static KeyCode toggleLandingGear = KeyCode.G;
        public static KeyCode toggleAirbrake = KeyCode.B;

        public static KeyCode throttleUpKey = KeyCode.LeftShift;
        public static KeyCode throttleDownKey = KeyCode.LeftControl;

        public static KeyCode camLand = KeyCode.Alpha0;
        public static KeyCode cam1 = KeyCode.Alpha1;
        public static KeyCode cam2 = KeyCode.Alpha2;
        public static KeyCode cam3 = KeyCode.Alpha3;

        public bool engineIngite = false;
        public bool rollLeft = false;
        public bool rollRight = false;
        public bool pitchUp = false;
        public bool pitchDown = false;
        public bool yawLeft = false;
        public bool yawRight = false;
        public bool throttleUp = false;
        public bool throttleDown = false;

        public Dictionary<KeyCode, bool> keys = new Dictionary<KeyCode, bool>();

        private void Start()
        {
            keys.Add(KeyCode.Space, engineIngite);
            keys.Add(KeyCode.Q, rollLeft);
            keys.Add(KeyCode.E, rollRight);
            keys.Add(KeyCode.A, yawLeft);
            keys.Add(KeyCode.D, yawRight);
            keys.Add(KeyCode.S, pitchUp);
            keys.Add(KeyCode.W, pitchDown);
        }

        public void Update()
        {
            if (Input.GetKeyDown(ignite))
            {
                engineIngite = !engineIngite;
            }

            if(Input.GetKeyDown(rotateZLeft))
            {
                rollLeft = true;
            }
            else if (Input.GetKeyUp(rotateZLeft))
            {
                rollLeft = false;
            }

            if (Input.GetKeyDown(rotateZRight))
            {
                rollRight = true;
            }
            else if (Input.GetKeyUp(rotateZRight))
            {
                rollRight = false;
            }

            if (Input.GetKeyDown(rotateYLeft))
            {
                yawLeft = true;
            }
            else if (Input.GetKeyUp(rotateYLeft))
            {
                yawLeft = false;
            }

            if (Input.GetKeyDown(rotateYRight))
            {
                yawRight = true;
            }
            else if (Input.GetKeyUp(rotateYRight))
            {
                yawRight = false;
            }

            if (Input.GetKeyDown(rotateUp))
            {
                pitchUp = true;
            }
            else if (Input.GetKeyUp(rotateUp))
            {
                pitchUp = false;
            }

            if (Input.GetKeyDown(rotateDown))
            {
                pitchDown = true;
            }
            else if (Input.GetKeyUp(rotateDown))
            {
                pitchDown = false;
            }

            if (Input.GetKeyDown(throttleUpKey))
            {
                throttleUp = true;
            }
            else if (Input.GetKeyUp(throttleUpKey))
            {
                throttleUp = false;
            }

            if (Input.GetKeyDown(throttleDownKey))
            {
                throttleDown = true;
            }
            else if (Input.GetKeyUp(throttleDownKey))
            {
                throttleDown = false;
            }
        }
    }
}
