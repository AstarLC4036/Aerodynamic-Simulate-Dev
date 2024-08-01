using System.Collections.Generic;
using Flight;
using Assets.Script.Game;
using BA.Keybinding;
using BA.Utility;
using UnityEngine;
using BA.FlightPhysics;
using NUnit.Framework;

namespace BA.Flight
{
    public class Aircraft : MonoBehaviour
    {
        [Header("Basic Properties")]
        public WorldGlobalData worldGlobalData;
        public KeyBinding key;

        public ParticleHolder flameHolder;
        public Animator landingGear;
        public Animator airbrake;
        public float engineThorttle = 1;
        public float engineThurst = 100;
        public float rotateTorque = 100;
        public bool engineOn = false;

        [Header("Control")]
        public bool isControlling = true;
        public Vector3 controlInput = Vector3.zero;

        public Transform rollRight;
        public Transform rollLeft;

        public Transform pitchLeft;
        public Transform pitchRight;

        public Transform yawLeft;
        public Transform yawRight;

        [Header("Physics")]
        public Vector3 centerOfMass = Vector3.zero;

        private float gForce;

        [Header("Areodynamics")]
        public AeroSurface[] surfaces;

        public AnimationCurve liftCurve;
        public AnimationCurve dragCurve;
        public AnimationCurve torqueCurve;

        public int maxFlapRotation = 20;
        public float surfaceRotateSpeed = 0.1f;

        public float airbrakeArea = 20;

        public float angleOfAttack, angleOfAttackYaw = 0;
        public float liftCoiffient, dragCoiffient, torqueCoiffient;
        private Vector3 localFlow, velocity;

        private Rigidbody aircraftRigidbody;
        private bool landingGearStowed = false;
        private bool airbrakeDeployed = false;

        private List<AeroSurface> controlSurfaces = new List<AeroSurface>();

        public float GForce
        {
            get
            {
                return gForce;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return aircraftRigidbody.velocity;
            }
        }

        public float AOA => angleOfAttack;

        // Use this for initialization
        void Start()
        {
            aircraftRigidbody = GetComponent<Rigidbody>();
            aircraftRigidbody.centerOfMass = centerOfMass;

            foreach(AeroSurface aeroSurface in surfaces)
            {
                if(aeroSurface.isControlSurface)
                {
                    controlSurfaces.Add(aeroSurface);
                }
            }
        }

        private void OnGUI()
        {
            //GUI.Label(new Rect(10, 10, 100, 20), "AOA: " + angleOfAttack);
            //GUI.Label(new Rect(10, 30, 100, 20), "Lift Coi:" + liftCoiffient);
            //GUI.Label(new Rect(10, 50, 100, 20), "Drag Coi:" + dragCoiffient);
        }

        private void Update()
        {
            if(isControlling)
                HandleInput();
        }

        void FixedUpdate()
        {
            CalcucateState();

            UpdateForces();

            UpdateControlSurfacesFixed();

            if (isControlling)
                HandleControl();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.1f, 0.1f, 1, 0.5f);
            Gizmos.DrawSphere(transform.position + centerOfMass, 1);
        }

        void CalcucateState()
        {
            velocity = aircraftRigidbody.velocity;
            localFlow = transform.InverseTransformVector(velocity);
            angleOfAttack = Mathf.Atan2(-localFlow.y, localFlow.x) * Mathf.Rad2Deg;

            liftCoiffient = liftCurve.Evaluate(angleOfAttack);
            dragCoiffient = dragCurve.Evaluate(angleOfAttack);
            torqueCoiffient = torqueCurve.Evaluate(angleOfAttack);
            //Debug.DrawLine(transform.position, transform.position + localFlow, Color.blue);
        }

        void UpdateForces()
        {
            //Aerodynamics forces(and torque);
            Vector3 totalLift = Vector3.zero;
            foreach (AeroSurface surface in surfaces)
            {
                if (!surface.gameObject.activeSelf || !surface.enabled)
                    continue;

                BiVector3 totalForces = surface.CalcucateForces(angleOfAttack, velocity, aircraftRigidbody.worldCenterOfMass);

                //LDMCoefficients coefficients = surface.CalcucateCoiffients(angleOfAttack);
                //BiVector3 liftAndDrag = surface.CalcucateForces(velocity, coefficients.liftCoefficient, coefficients.dragCoefficient);
                //Vector3 torque = surface.CalcucateTorque(velocity, liftAndDrag.lift + liftAndDrag.drag, aircraftRigidbody.worldCenterOfMass, coefficients.torqueCoefficient);
                aircraftRigidbody.AddForce(totalForces.lift);
                aircraftRigidbody.AddForce(totalForces.drag);
                aircraftRigidbody.AddTorque(totalForces.torque);
                Debug.DrawLine(surface.transform.position, surface.transform.position + totalForces.lift / 1000, Color.green);
                Debug.DrawLine(surface.transform.position, surface.transform.position + totalForces.drag / 1000, Color.red);
                //Debug.DrawLine(surface.transform.position, surface.transform.position + Vector3.Cross(aircraftRigidbody.worldCenterOfMass - surface.transform.position, totalForces.lift + totalForces.drag), Color.green);
                //Debug.DrawLine(surface.transform.position, surface.transform.position + totalForces.lift / 100, Color.green);
                //Debug.DrawLine(surface.transform.position, surface.transform.position + totalForces.drag / 100, Color.red);
                Debug.DrawLine(surface.transform.position, surface.transform.position + totalForces.torque / 1000, Color.blue);

                totalLift += totalForces.lift;
                CalcucateGForce(totalLift);
            }

            //airbrake drag
            if(airbrakeDeployed)
            {
                float dymaticPresure = 0.5f * 1.225f * aircraftRigidbody.velocity.sqrMagnitude;
                Vector3 airbrakeDrag = -aircraftRigidbody.velocity.normalized * (dymaticPresure * airbrakeArea * dragCurve.Evaluate(angleOfAttack + 90));
                aircraftRigidbody.AddForce(airbrakeDrag);
            }

            //engine thurst
            UpdateThurst();
        }

        void UpdateControlSurfacesFixed()
        {
            controlSurfaces.ForEach((AeroSurface surface) =>
            {
                if(surface.flapAngle != MathUtility.CovertAngle(surface.flapTransform.localEulerAngles.x) * Mathf.Deg2Rad || surface.TargetFlapAngle != surface.flapAngle)
                {
                    surface.UpdateControlSurface();
                }
            });
        }

        public void UpdateThurst()
        {
            if(engineOn)
            {
                aircraftRigidbody.AddForce(engineThurst * engineThorttle * transform.right);
            }
        }

        public void CalcucateGForce(Vector3 totalLift)
        {
            int gSign = 1;
            if(liftCoiffient >= 0)
            {
                gSign = 1;
            }
            else
            {
                gSign = -1;
            }
            gForce = (totalLift.magnitude / (aircraftRigidbody.mass * 9.8f)) * gSign;
        }

        void HandleInput()
        {
            //Landing Gear
            if (Input.GetKeyDown(KeyBinding.toggleLandingGear))
            {
                if (!landingGearStowed)
                {
                    landingGear.SetBool("StowLandingGear", true);
                    landingGear.SetBool("LowerLandingGear", false);
                }
                else
                {
                    landingGear.SetBool("StowLandingGear", false);
                    landingGear.SetBool("LowerLandingGear", true);
                }
                landingGearStowed = !landingGearStowed;
            }

            /*
            if (Input.GetKeyDown(KeyBinding.toggleLandingGear))
            {
                lg.Play();
            }
            */

                //Airbrake
            if (Input.GetKeyDown(KeyBinding.toggleAirbrake))
            {
                if (!airbrakeDeployed)
                {
                    airbrake.SetBool("depolyAirbrake", true);
                }
                else
                {
                    airbrake.SetBool("depolyAirbrake", false);
                }
                airbrakeDeployed = !airbrakeDeployed;
            }
        }

        void HandleControl()
        {
            if (key.engineIngite)
            {
                engineOn = true;
                if (!flameHolder.isPlaying && engineThorttle >= 0.7f)
                {
                    flameHolder.PlayParticle();
                }
                else if(flameHolder.isPlaying && engineThorttle < 0.7f)
                {
                    flameHolder.StopParticle();
                }
            }
            else if (flameHolder.isPlaying)
            {
                engineOn = false;
                flameHolder.StopParticle();
            }

            if(engineOn)
            {
                float lifeTime = 0.4f + (engineThorttle - 0.7f) / 0.3f;
                foreach (ParticleSystem particleSystem in flameHolder.particleSystems)
                {
                    if (particleSystem.startLifetime != lifeTime)
                    {
                        particleSystem.startLifetime = lifeTime;
                    }
                }
            }

            //Throttle
            if (key.throttleUp && !key.throttleDown)
            {
                if (engineThorttle < 1)
                    engineThorttle += 0.01f;
                if (engineThorttle > 1)
                    engineThorttle = 1;
            }
            else if (key.throttleDown && !key.throttleUp)
            {
                if (engineThorttle > 0)
                    engineThorttle -= 0.01f;
                if (engineThorttle < 0)
                    engineThorttle = 0;
            }

            //Yaw & Pitch & Roll
            //横滚(向右)
            if (key.rollLeft && !key.rollRight)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(rotateTorque, 0, 0), ForceMode.VelocityChange);
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if(surface.surfaceType == ControlSurfaceType.Roll && surface.TargetFlapAngle != -surface.maxFlapAngle)
                        surface.SetTargetFlapAngle(-surface.maxFlapAngle);
                });

            }
            //横滚(向左)
            else if (!key.rollLeft && key.rollRight)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(-rotateTorque, 0, 0), ForceMode.VelocityChange);
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if (surface.surfaceType == ControlSurfaceType.Roll && surface.TargetFlapAngle != surface.maxFlapAngle)
                        surface.SetTargetFlapAngle(surface.maxFlapAngle);
                });
            }
            //回正
            else
            {
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if (surface.surfaceType == ControlSurfaceType.Roll && surface.TargetFlapAngle != 0)
                        surface.SetTargetFlapAngle(0);
                });
            }

            //俯仰(向下)
            if (key.pitchUp && !key.pitchDown)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, 0, rotateTorque), ForceMode.VelocityChange);
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if (surface.surfaceType == ControlSurfaceType.Pitch && surface.TargetFlapAngle != surface.maxFlapAngle)
                        surface.SetTargetFlapAngle(-surface.maxFlapAngle);
                });
            }
            //俯仰(向上)
            else if (!key.pitchUp && key.pitchDown)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, 0, -rotateTorque), ForceMode.VelocityChange);
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if (surface.surfaceType == ControlSurfaceType.Pitch && surface.TargetFlapAngle != -surface.maxFlapAngle)
                        surface.SetTargetFlapAngle(surface.maxFlapAngle);
                });
            }
            //回正
            else
            {
                controlSurfaces.ForEach((AeroSurface surface) =>
                {
                    if (surface.surfaceType == ControlSurfaceType.Pitch && surface.TargetFlapAngle != 0)
                        surface.SetTargetFlapAngle(0);
                });
            }
            
            //偏航左旋转
            if (key.yawLeft && !key.yawRight)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, -rotateTorque, 0), ForceMode.VelocityChange);
                controlInput.y = -1;
                if (yawLeft.localEulerAngles.y > 360 - maxFlapRotation || yawLeft.localEulerAngles.y <= maxFlapRotation)
                    yawLeft.Rotate(0, -surfaceRotateSpeed, 0, Space.Self);
                if (yawRight.localEulerAngles.y > 360 - maxFlapRotation || yawRight.localEulerAngles.y <= maxFlapRotation)
                    yawRight.Rotate(0, -surfaceRotateSpeed, 0, Space.Self);
            }
            //偏航右旋转
            else if (!key.yawLeft && key.yawRight)
            {
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, rotateTorque, 0), ForceMode.VelocityChange);
                controlInput.y = 1;
                if (yawLeft.localEulerAngles.y < maxFlapRotation || yawLeft.localEulerAngles.y >= 360 - maxFlapRotation)
                    yawLeft.Rotate(0, surfaceRotateSpeed, 0, Space.Self);
                if (yawRight.localEulerAngles.y < maxFlapRotation || yawRight.localEulerAngles.y >= 360 - maxFlapRotation)
                    yawRight.Rotate(0, surfaceRotateSpeed, 0, Space.Self);
            }
            //回正
            else
            {
                if (yawLeft.localEulerAngles.y <= surfaceRotateSpeed * 1.5 || yawLeft.localEulerAngles.y >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    yawLeft.localRotation = Quaternion.Euler(0, 0, 0);
                    controlInput.y = 0;
                }
                else if (yawLeft.localEulerAngles.y > 0 && yawLeft.localEulerAngles.y < 180)
                    yawLeft.Rotate(0, -surfaceRotateSpeed, 0);
                else if (yawLeft.localEulerAngles.y >= 180 && yawLeft.localEulerAngles.y < 360)
                    yawLeft.Rotate(0, surfaceRotateSpeed, 0);

                if (yawRight.localEulerAngles.y <= surfaceRotateSpeed * 1.5f || yawRight.localEulerAngles.y >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    yawRight.localRotation = Quaternion.Euler(0, 0, 0);
                    controlInput.y = 0;
                }
                else if (yawRight.localEulerAngles.y > 0 && yawRight.localEulerAngles.y < 180)
                    yawRight.Rotate(0, -surfaceRotateSpeed, 0);
                else if (yawRight.localEulerAngles.y >= 180 && yawRight.localEulerAngles.y < 360)
                    yawRight.Rotate(0, surfaceRotateSpeed, 0);
            }

            /*
            //Old code
            
            //R1
                aircraftRigidbody.AddRelativeTorque(new Vector3(rotateTorque, 0, 0), ForceMode.VelocityChange);
                controlInput.x = 1;
                if (rollLeft.localEulerAngles.x <= maxFlapRotation || rollLeft.localEulerAngles.x > 360 - maxFlapRotation)
                    rollLeft.Rotate(-surfaceRotateSpeed, 0, 0);
                if (rollRight.localEulerAngles.x < maxFlapRotation || rollRight.localEulerAngles.x >= 360 - maxFlapRotation)
                    rollRight.Rotate(surfaceRotateSpeed, 0, 0);

            //R2
                aircraftRigidbody.AddRelativeTorque(new Vector3(-rotateTorque, 0, 0), ForceMode.VelocityChange);
                controlInput.x = -1;
                if (rollLeft.localEulerAngles.x < maxFlapRotation || rollLeft.localEulerAngles.x >= 360 - maxFlapRotation)
                    rollLeft.Rotate(surfaceRotateSpeed, 0, 0);
                if (rollRight.localEulerAngles.x <= maxFlapRotation || rollRight.localEulerAngles.x > 360 - maxFlapRotation)
                    rollRight.Rotate(-surfaceRotateSpeed, 0, 0);

            //RB
                if (rollLeft.localEulerAngles.x <= surfaceRotateSpeed * 1.5 || rollLeft.localEulerAngles.x >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    rollLeft.localRotation = Quaternion.Euler(0, 0, 0);
                    controlInput.x = 0;
                }
                else if (rollLeft.localEulerAngles.x > 0 && rollLeft.localEulerAngles.x < 180)
                    rollLeft.Rotate(-surfaceRotateSpeed, 0, 0);
                else if (rollLeft.localEulerAngles.x >= 180 && rollLeft.localEulerAngles.x < 360)
                    rollLeft.Rotate(surfaceRotateSpeed, 0, 0);

                if (rollRight.localEulerAngles.x <= surfaceRotateSpeed * 1.5f || rollRight.localEulerAngles.x >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    controlInput.x = 0;
                    rollRight.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (rollRight.localEulerAngles.x > 0 && rollRight.localEulerAngles.x < 180)
                    rollRight.Rotate(-surfaceRotateSpeed, 0, 0);
                else if (rollRight.localEulerAngles.x >= 180 && rollRight.localEulerAngles.x < 360)
                    rollRight.Rotate(surfaceRotateSpeed, 0, 0);

            //P1
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, 0, rotateTorque), ForceMode.VelocityChange);
                controlInput.z = 1;
                if (pitchLeft.localEulerAngles.x < maxFlapRotation || pitchLeft.localEulerAngles.x >= 360 - maxFlapRotation)
                    pitchLeft.Rotate(surfaceRotateSpeed, 0, 0);
                if (pitchRight.localEulerAngles.x < maxFlapRotation || pitchRight.localEulerAngles.x >= 360 - maxFlapRotation)
                    pitchRight.Rotate(surfaceRotateSpeed, 0, 0);

            //P2
                aircraftRigidbody.AddRelativeTorque(new Vector3(0, 0, -rotateTorque), ForceMode.VelocityChange);
                controlInput.z = -1;
                if (pitchLeft.localEulerAngles.x <= maxFlapRotation || pitchLeft.localEulerAngles.x > 360 - maxFlapRotation)
                    pitchLeft.Rotate(-surfaceRotateSpeed, 0, 0);
                if (pitchRight.localEulerAngles.x <= maxFlapRotation || pitchRight.localEulerAngles.x > 360 - maxFlapRotation)
                    pitchRight.Rotate(-surfaceRotateSpeed, 0, 0);

            //PB
                if (pitchLeft.localEulerAngles.x <= surfaceRotateSpeed * 1.5 || pitchLeft.localEulerAngles.x >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    pitchLeft.localRotation = Quaternion.Euler(0, 0, 0);
                    controlInput.z = 0;
                }
                else if (pitchLeft.localEulerAngles.x > 0 && pitchLeft.localEulerAngles.x < 180)
                    pitchLeft.Rotate(-surfaceRotateSpeed, 0, 0);
                else if (pitchLeft.localEulerAngles.x >= 180 && pitchLeft.localEulerAngles.x < 360)
                    pitchLeft.Rotate(surfaceRotateSpeed, 0, 0);

                if (pitchRight.localEulerAngles.x <= surfaceRotateSpeed * 1.5f || pitchRight.localEulerAngles.x >= 360 - surfaceRotateSpeed * 1.5f)
                {
                    pitchRight.localRotation = Quaternion.Euler(0, 0, 0);
                    controlInput.z = 0;
                }
                else if (pitchRight.localEulerAngles.x > 0 && pitchRight.localEulerAngles.x < 180)
                    pitchRight.Rotate(-surfaceRotateSpeed, 0, 0);
                else if (pitchRight.localEulerAngles.x >= 180 && pitchRight.localEulerAngles.x < 360)
                    pitchRight.Rotate(surfaceRotateSpeed, 0, 0);
            */
        }
    }
}