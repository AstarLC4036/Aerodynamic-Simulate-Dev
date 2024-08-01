using BA.Flight;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

namespace Assets.Script.Flight
{
    public class Missle : MonoBehaviour
    {
        public Rigidbody aircraftRB;
        public Aircraft targetAircraft;
        public MissleExplodeRadius explodeRadius;
        public ParticleSystem engineFlame;
        public ParticleSystem explode;
        public float igniteDuration = 0.1f;
        public float mass = 1000;
        public float thurst = 1000;
        public float fuel = 1000;
        public float fuelReduce = 10;
        public float rotateAlpha = 0.1f;

        private Rigidbody rb;
        private Transform targetPointer;
        private Transform target;
        private bool ignite = false;
        private bool startIgnite = false;
        private bool launched = false;
        private float launchTime = 0;
        private Vector3 hitPoint = Vector3.zero;

        public void Launch()
        {
            launched = true;

            Destroy(transform.parent.GetComponent<FixedJoint>());
            rb = transform.parent.GetComponent<Rigidbody>();
            rb.mass = mass;
            rb.centerOfMass = Vector3.zero;
            rb.useGravity = true;
            rb.drag = 0.3f;
            rb.angularDrag = 0.5f;

            GameObject obj = new GameObject("Target Pointer");
            obj.transform.SetParent(transform.parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.SetParent(transform);
            targetPointer = obj.transform;
        }

        public void Init(Aircraft target)
        {
            targetAircraft = target;

        }

        public void Start()
        {
            target = targetAircraft.transform;
        }

        public void FixedUpdate()
        {
            if (!ignite && launched)
            {
                if (launchTime < igniteDuration)
                {
                    launchTime += Time.fixedDeltaTime;
                    if (launchTime > igniteDuration)
                        launchTime = igniteDuration;
                }
                else
                {
                    ignite = true;
                    startIgnite = true;
                }
            }

            if(startIgnite)
            {
                rb.useGravity = false;
                rb.velocity = aircraftRB.velocity;
                startIgnite = false;

                engineFlame.Play();
            }

            if (!ignite)
                return;


            Vector3 D = transform.position - target.position;
            Vector3 aircraftDirection = target.forward;
            float theta = Vector3.Angle(D, aircraftDirection);
            float DD = D.magnitude;
            float A = 1 - Mathf.Pow((rb.velocity.magnitude / targetAircraft.Velocity.magnitude), 2);
            float B = -(2 * DD * Mathf.Cos(theta * Mathf.Deg2Rad));
            float C = DD * DD;
            float delta = B * B - 4 * A * C;
            if(delta >= 0)
            {
                float x1 = (-B + Mathf.Sqrt(delta)) / (2 * A);
                float x2 = (-B - Mathf.Sqrt(delta)) / (2 * A);

                if(x1 < x2)
                {
                    x1 = x2;
                }
                hitPoint = target.position + aircraftDirection * x1;

                transform.LookAt(hitPoint);
            }
            //transform.parent.LookAt(target);
            //float rotationAlpha = Mathf.Abs((targetPointer.transform.eulerAngles - transform.eulerAngles).magnitude) <= rotateAlpha ? 1 : rotateAlpha;
            //Quaternion nextRotation = Quaternion.Lerp(transform.rotation, targetPointer.transform.rotation, rotationAlpha);
            //rb.MoveRotation(nextRotation);
            //rb.MoveRotation(Quaternion.Euler(target.eulerAngles));

            if (fuel > 0)
            {
                rb.AddForce(transform.forward * thurst);
                fuel -= fuelReduce * Time.fixedDeltaTime;

                if (fuel < 0)
                {
                    fuel = 0;
                }
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(hitPoint, 5);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!launched)
                return;
            explode.transform.SetParent(null);
            explode.Play();
            Destroy(other.gameObject);
            Destroy(gameObject);
            explodeRadius.AddForce();
        }
    }
}