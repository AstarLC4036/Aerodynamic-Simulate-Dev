using BA.Flight;
using BA.FlightPhysics;
using BA.Game;
using BA.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Flight
{
    //References : 
    //(ResearchGate)Real-time modeling of agile fixed-wing UAV aerodynamics
    //(GitHub: gasgiant)Aircraft-Physics
    public class AeroSurface : MonoBehaviour
    {
        [Header("Data")]
        public AeroSurfaceData surfaceData;

        [Header("Surface Settings")]
        public float flapAngle = 0;
        public float maxFlapAngle = 0;
        public float flapMoveSpeed = 0;
        public bool isControlSurface = false;
        [HideInInspector]
        public ControlSurfaceType surfaceType = ControlSurfaceType.None;
        [HideInInspector]
        public bool invertInput = false;
        [HideInInspector]
        public Transform flapTransform;

        public Vector3 coefficientsInfo;

        //[SerializeField]
        private Vector3 sizeFixed = Vector3.zero;
        private Vector3 flapSizeFixed = Vector3.zero;

        private Mesh surfaceMesh;
        private Mesh flapMesh;

        [SerializeField]
        private float targetFlapAngle;

        private Material gl_mat;

        public float TargetFlapAngle
        {
            get
            {
                return targetFlapAngle;
            }
        }

        private void Start()
        {
            if (gameObject.activeSelf && enabled)
                UpdateData();
        }

        private void OnValidate()
        {
            if (gameObject.activeSelf && enabled)
                UpdateData();
        }

        private void FixedUpdate()
        {
            /*
            if(flapAngle < targetFlapAngle)
            {
                flapAngle += flapMoveSpeed * Time.fixedDeltaTime;
                if (flapAngle > flapMoveSpeed)
                {
                    flapAngle = targetFlapAngle;
                }
            }
            else if (flapAngle > targetFlapAngle)
            {
                flapAngle -= flapMoveSpeed * Time.fixedDeltaTime;
                if (flapAngle < flapMoveSpeed)
                {
                    flapAngle = targetFlapAngle;
                }
            }
            */
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawMesh(surfaceMesh, 0, transform.position, transform.rotation);
            Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.DrawMesh(flapMesh, 0, transform.position, transform.rotation);
        }

        public void UpdateData()
        {
            if(surfaceData == null)
            {
                return;
            }

            float chordSurface = surfaceData.chord * (1 - surfaceData.flapPercent);
            float chordFlap = surfaceData.chord * surfaceData.flapPercent;
            GenerateSurfaceMesh(new Vector3(surfaceData.span, 0, chordSurface), chordFlap / 2, ref surfaceMesh);
            GenerateSurfaceMesh(new Vector3(surfaceData.span, 0, chordFlap), chordSurface / -2, ref flapMesh);

            /*
            sizeFixed.x = surfaceData.span;
            sizeFixed.z = surfaceData.chord * (1 - surfaceData.flapPercent);

            surfaceMesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
                new Vector3(-sizeFixed.x / 2, sizeFixed.y / 2, sizeFixed.z / 2),
                new Vector3(sizeFixed.x / 2,  sizeFixed.y / 2, sizeFixed.z / 2),
                new Vector3(-sizeFixed.x / 2, sizeFixed.y / 2, -sizeFixed.z / 2),
                new Vector3(sizeFixed.x / 2,  sizeFixed.y / 2, -sizeFixed.z / 2),
                new Vector3(-sizeFixed.x / 2, -sizeFixed.y / 2, sizeFixed.z / 2),
                new Vector3(sizeFixed.x / 2,  -sizeFixed.y / 2, sizeFixed.z / 2),
                new Vector3(-sizeFixed.x / 2, -sizeFixed.y / 2, -sizeFixed.z / 2),
                new Vector3(sizeFixed.x / 2,  -sizeFixed.y / 2, -sizeFixed.z / 2)
            };
            //int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, 0, 2, 6, 0, 6, 4, 0, 5, 1, 0, 4, 5, 1, 7, 3, 1, 5, 7, 2, 3, 7, 2, 7, 6, 4, 6, 5, 5, 6, 7 };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            surfaceMesh.vertices = vertices;
            surfaceMesh.triangles = triangles;
            surfaceMesh.RecalculateNormals();
            */
        }

        public void GenerateSurfaceMesh(Vector3 size, float offset, ref Mesh mesh)
        {
            mesh = new Mesh();
            Vector3[] vertices = new Vector3[] {
                new Vector3(-size.x / 2, size.y / 2, size.z / 2 + offset),
                new Vector3(size.x / 2,  size.y / 2, size.z / 2 + offset),
                new Vector3(-size.x / 2, size.y / 2, -size.z / 2 + offset),
                new Vector3(size.x / 2,  size.y / 2, -size.z / 2 + offset),
                new Vector3(-size.x / 2, -size.y / 2, size.z / 2 + offset),
                new Vector3(size.x / 2,  -size.y / 2, size.z / 2 + offset),
                new Vector3(-size.x / 2, -size.y / 2, -size.z / 2 + offset),
                new Vector3(size.x / 2,  -size.y / 2, -size.z / 2 + offset)
            };
            //int[] triangles = new int[] { 0, 1, 2, 1, 3, 2, 0, 2, 6, 0, 6, 4, 0, 5, 1, 0, 4, 5, 1, 7, 3, 1, 5, 7, 2, 3, 7, 2, 7, 6, 4, 6, 5, 5, 6, 7 };
            int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        public BiVector3 CalcucateForces(float angleOfAttack, Vector3 velocity, Vector3 aircraftCenterOfMass)
        {
            LDMCoefficients coefficients = CalcucateCoiffients(angleOfAttack);

            float dymaticPresure = 0.5f * 1.225f * velocity.sqrMagnitude;
            float area = surfaceData.area;

            float liftForce = dymaticPresure * coefficients.liftCoefficient * area;
            Vector3 lift = liftForce * transform.up;
            //Vector3 lift = liftForce * Vector3.Cross(-velocity.normalized, -rightAxis);

            if(coefficients.dragCoefficient < 0.01f)
            {
                coefficients.dragCoefficient = 0.01f;
            }

            float dragForce = dymaticPresure * coefficients.dragCoefficient * area;
            Vector3 drag = dragForce * -velocity.normalized;

            Vector3 torque = transform.right * coefficients.torqueCoefficient * dymaticPresure * area * surfaceData.chord;
            Vector3 totalTorque = Vector3.Cross(transform.position - aircraftCenterOfMass, lift + drag) + torque;

            return new BiVector3(lift, drag, totalTorque);
        }
        public Vector3 CalcucateLift(Vector3 velocity, float LC)
        {
            float dymaticPresure = 0.5f * 1.225f * velocity.sqrMagnitude;
            float area = surfaceData.area;
            float liftForce = dymaticPresure * LC * area;
            Vector3 lift = liftForce * transform.up;
            return lift;
        }

        public Vector3 CalcucateDrag(Vector3 velocity, float DC)
        {
            float dymaticPresure = 0.5f * 1.225f * velocity.sqrMagnitude;
            float area = surfaceData.area;
            float dragForce = dymaticPresure * DC * area;
            Vector3 drag = dragForce * -velocity.normalized;
            return drag;
        }

        public BiVector3 CalcucateForces(Vector3 velocity, float LC, float DC)
        {
            Vector3 lift = CalcucateLift(velocity, LC);
            Vector3 drag = CalcucateDrag(velocity, DC);

            return new BiVector3(lift, drag);
        }

        public Vector3 CalcucateTorque(Vector3 velocity, Vector3 totalForce, Vector3 worldCenterOfMass, float TC)
        {
            float dymaticPresure = 0.5f * 1.225f * velocity.sqrMagnitude;
            float area = surfaceData.area;
            Vector3 torque = transform.right * TC * dymaticPresure * area * surfaceData.chord;
            return Vector3.Cross(transform.position - worldCenterOfMass, totalForce) + torque;
        }

        public void SetTargetFlapAngle(float flapAngle)
        {
            if (invertInput)
                targetFlapAngle = -flapAngle;
            else
                targetFlapAngle = flapAngle;
        }

        public void UpdateControlSurface()
        {
            float flapAngleDeg = flapAngle * Mathf.Rad2Deg;

            if (flapAngleDeg < targetFlapAngle)
            {
                flapAngleDeg += flapMoveSpeed * Time.fixedDeltaTime;
                if(flapAngleDeg > targetFlapAngle)
                {
                    flapAngleDeg = targetFlapAngle;
                }
            }
            else if(flapAngleDeg > targetFlapAngle)
            {
                flapAngleDeg -= flapMoveSpeed * Time.fixedDeltaTime;
                if (flapAngleDeg < targetFlapAngle)
                {
                    flapAngleDeg = targetFlapAngle;
                }
            }

            flapAngle = flapAngleDeg * Mathf.Deg2Rad;

            flapTransform.localRotation = Quaternion.Euler(flapAngleDeg, flapTransform.localEulerAngles.y, flapTransform.localEulerAngles.z);

            /*
            float surfaceAngle = MathUtility.CovertAngle(flapTransform.localEulerAngles.x);
            if (surfaceAngle < targetFlapAngle)
            {
                flapTransform.Rotate(flapMoveSpeed * Time.fixedDeltaTime, 0, 0, Space.Self);
                if(surfaceAngle > targetFlapAngle)
                {
                    flapTransform.localRotation = Quaternion.Euler(targetFlapAngle, flapTransform.localEulerAngles.y, flapTransform.localEulerAngles.z);
                }
            }
            else if(surfaceAngle > targetFlapAngle)
            {
                flapTransform.Rotate(-flapMoveSpeed * Time.fixedDeltaTime, 0, 0, Space.Self);
                if (surfaceAngle < targetFlapAngle)
                {
                    flapTransform.localRotation = Quaternion.Euler(targetFlapAngle, flapTransform.localEulerAngles.y, flapTransform.localEulerAngles.z);
                }
            }
            else if(surfaceAngle <= 0.1 && surfaceAngle >= -0.1)
            {
                flapTransform.localRotation = Quaternion.Euler(0, flapTransform.localEulerAngles.y, flapTransform.localEulerAngles.z);
            }

            flapAngle = MathUtility.CovertAngle(flapTransform.localEulerAngles.x) * Mathf.Deg2Rad;
            */
        }

        //Real-time Aerodynamics Modeling
        public LDMCoefficients CalcucateCoiffients(float aoa)
        {
            LDMCoefficients coefficients;

            float aoaRad = aoa * Mathf.Deg2Rad;

            float AR = surfaceData.AR;
            float correctedLiftSlope = surfaceData.liftSlope * (AR / (AR + 2 * (AR + 4) / (AR + 2)));//CLa
            float flapFraction = surfaceData.flapPercent; //Equals surfaceData.chord * surfaceData.flapPercent(flap chord) / surfaceData.chord
            float theta = Mathf.Acos(2 * flapFraction - 1);
            float deltaCL = correctedLiftSlope * (1 - (theta - Mathf.Sin(theta)) / Mathf.PI) * Mathf.Lerp(0.8f, 0.4f, (Mathf.Abs(flapAngle) * Mathf.Rad2Deg - 10) / 50) * flapAngle;
            float deltaCLMax = deltaCL * Mathf.Clamp01(1 - 0.5f * (flapFraction - 0.1f) / 0.3f);
            float CLMaxP = correctedLiftSlope * (surfaceData.aoaStallPB * Mathf.Deg2Rad - surfaceData.zeroLiftAngle * Mathf.Deg2Rad) + deltaCLMax; // P means positive
            float CLMaxN = correctedLiftSlope * (surfaceData.aoaStallNB * Mathf.Deg2Rad - surfaceData.zeroLiftAngle * Mathf.Deg2Rad) + deltaCLMax; // N means negative
            float aoaZero = surfaceData.zeroLiftAngle * Mathf.Deg2Rad - deltaCL / correctedLiftSlope; 
            float aoaStallP = aoaZero + CLMaxP / correctedLiftSlope;
            float aoaStallN = aoaZero + CLMaxN / correctedLiftSlope;

            float paddingAngleHigh = Mathf.Deg2Rad * Mathf.Lerp(15, 5, (Mathf.Rad2Deg * flapAngle + 50) / 100);
            float paddingAngleLow = Mathf.Deg2Rad * Mathf.Lerp(15, 5, (-Mathf.Rad2Deg * flapAngle + 50) / 100);
            float paddedStallAngleHigh = aoaStallP + paddingAngleHigh;
            float paddedStallAngleLow = aoaStallN - paddingAngleLow;

            //Low Angle of Attack Aerodynamics 
            if (aoaStallN < aoaRad && aoaRad < aoaStallP) // aStall,N < a < aStall,P
            {
                LDMCoefficients coef = CalculateCoefficientsAtLowAOA(aoaRad, aoaZero, correctedLiftSlope, AR);

                coefficientsInfo = coef.ToVector3();
                coefficients = coef;
            }

            //High Angle of Attack Aerodynamics 
            else
            {
                if (aoaRad > paddedStallAngleHigh || aoaRad < paddedStallAngleLow) //aoa < aStall,N or aStall,P < aoa
                {
                    //Rewrited in a func.

                    LDMCoefficients coef = CalculateCoefficientsAtHighAOA(aoaRad, aoaStallP, aoaStallN, aoaZero, correctedLiftSlope, AR);

                    coefficientsInfo = coef.ToVector3();
                    coefficients = coef;
                }
                else
                {
                    // Linear stitching in-between stall and low angles of attack modes.
                    Vector3 aerodynamicCoefficientsLow;
                    Vector3 aerodynamicCoefficientsStall;
                    float lerpParam;

                    if (aoaRad > aoaStallP)
                    {
                        aerodynamicCoefficientsLow = CalculateCoefficientsAtLowAOA(aoaStallP, aoaZero, correctedLiftSlope, AR).ToVector3();
                        aerodynamicCoefficientsStall = CalculateCoefficientsAtHighAOA(
                            paddedStallAngleHigh, aoaStallP, aoaStallN, aoaZero, correctedLiftSlope, AR).ToVector3();
                        lerpParam = (aoaRad - aoaStallP) / (paddedStallAngleHigh - aoaStallP);
                    }
                    else
                    {
                        aerodynamicCoefficientsLow = CalculateCoefficientsAtLowAOA(aoaStallN, aoaZero, correctedLiftSlope, AR).ToVector3();
                        aerodynamicCoefficientsStall = CalculateCoefficientsAtHighAOA(
                            paddedStallAngleLow, aoaStallP, aoaStallN, aoaZero, correctedLiftSlope, AR).ToVector3();
                        lerpParam = (aoaRad - aoaStallN) / (paddedStallAngleLow - aoaStallN);
                    }
                    coefficients = LDMCoefficients.FromVector3(Vector3.Lerp(aerodynamicCoefficientsLow, aerodynamicCoefficientsStall, lerpParam));
                }
            }

            return coefficients;
        }

        LDMCoefficients CalculateCoefficientsAtLowAOA(float aoaRad, float aoaZero, float correctedLiftSlope, float AR)
        {
            float CL = correctedLiftSlope * (aoaRad - aoaZero); //Lift Coefficient
            float aInd = CL / (Mathf.PI * AR); //Induced AoA(诱导迎角)
            float aEff = aoaRad - aoaZero - aInd; //Effective Angle
            float CT = surfaceData.skinFrictionCoiffient * Mathf.Cos(aEff); //Tangential Coefficient
            float CN = (CL + CT * Mathf.Sin(aEff)) / Mathf.Cos(aEff); //Normal Coefficient
            float CD = CN * Mathf.Sin(aEff) + CT * Mathf.Cos(aEff); //Drag Coefficient
            float CM = -CN * (0.25f - 0.175f * (1 - 2 * Mathf.Abs(aEff) / Mathf.PI)); //Torque Coefficient

            return new LDMCoefficients(CL, CD, CM);
        }

        LDMCoefficients CalculateCoefficientsAtHighAOA(float aoaRad, float aoaStallP, float aoaStallN, float aoaZero, float correctedLiftSlope, float AR)
        {
            float CdZero = surfaceData.skinFrictionCoiffient;

            float liftCoefficientLowAoA;
            if (aoaRad > aoaStallP)
            {
                liftCoefficientLowAoA = correctedLiftSlope * (aoaStallP - aoaZero);
            }
            else
            {
                liftCoefficientLowAoA = correctedLiftSlope * (aoaStallN - aoaZero);
            }
            float inducedAngle = liftCoefficientLowAoA / (Mathf.PI * AR);

            float lerpParam;
            if (aoaRad > aoaStallP)
            {
                lerpParam = (Mathf.PI / 2 - Mathf.Clamp(aoaRad, -Mathf.PI / 2, Mathf.PI / 2))
                    / (Mathf.PI / 2 - aoaStallP);
            }
            else
            {
                lerpParam = (-Mathf.PI / 2 - Mathf.Clamp(aoaRad, -Mathf.PI / 2, Mathf.PI / 2))
                    / (-Mathf.PI / 2 - aoaStallN);
            }
            inducedAngle = Mathf.Lerp(0, inducedAngle, lerpParam);
            float effectiveAngle = aoaRad - aoaZero - inducedAngle;

            float CdN = -4.26e-2f * flapAngle * flapAngle + 2.1e-1f * flapAngle + 1.98f; // Cd,90
            float normalCoefficient = CdN * Mathf.Sin(effectiveAngle) *
                (1 / (0.56f + 0.44f * Mathf.Abs(Mathf.Sin(effectiveAngle))) -
                0.41f * (1 - Mathf.Exp(-17 / AR)));
            float tangentialCoefficient = 0.5f * CdZero * Mathf.Cos(effectiveAngle);

            float liftCoefficient = normalCoefficient * Mathf.Cos(effectiveAngle) - tangentialCoefficient * Mathf.Sin(effectiveAngle);
            float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAngle) + tangentialCoefficient * Mathf.Cos(effectiveAngle);
            float torqueCoefficient = -normalCoefficient * Mathf.Abs(0.25f - 0.175f * (1 - 2 * effectiveAngle / Mathf.PI));

            //torqueCoefficient = Mathf.Abs(0.25f - 0.175f * (1 - 2 * effectiveAngle / Mathf.PI));
            //torqueCoefficient = -normalCoefficient;

            return new LDMCoefficients(liftCoefficient, dragCoefficient, torqueCoefficient);

            //Origin code
            /*
            float aStall = 0;
            float aInd = 0; //induced aoa
            if (aoaStallN >= aoaRad) //Stall(High aoa) & aoa < 0
            {
                aStall = aoaStallN;
                float CLStall = correctedLiftSlope * (aStall - aoaZero);
                float aIndStall = CLStall / (Mathf.PI * AR);
                aInd = Mathf.Lerp(aIndStall, 0, (aStall - aoaRad) /(aStall + 90));
            }
            else //Stall(High aoa) & aoa > 0
            {
                aStall = aoaStallP;
                float CLStall = correctedLiftSlope * (aStall - aoaZero);
                float aIndStall = CLStall / (Mathf.PI * AR);
                aInd = Mathf.Lerp(aIndStall, 0, (aoaRad - aStall)/(90 - aStall));
            }
            
            float aEff = aoaRad - aoaZero - aInd;
            float CdN = -4.26e-2f * flapAngle * flapAngle + 2.1e-1f * flapAngle + 1.98f; // Cd,90
            float CN = CdN * Mathf.Sin(aEff) * (1 / (0.5f + 0.44f * Mathf.Sin(aEff)) - 0.41f * (1 - Mathf.Exp(-17 / AR)));
            float CT = 0.5f * surfaceData.skinFrictionCoiffient * Mathf.Cos(aEff);
            CL = CN * Mathf.Cos(aEff) - CT * Mathf.Sin(aEff);
            CD = CN * Mathf.Sin(aEff) + CT * Mathf.Cos(aEff);
            CM = -CN * (0.25f - 0.175f * (1 - 2 * aEff / Mathf.PI));
            */
        }
    }
}