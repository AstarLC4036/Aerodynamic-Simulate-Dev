using UnityEngine;

namespace BA.Flight
{
    public class WorldGlobalData : MonoBehaviour
    {
        public static Vector3 wind = new Vector3(0, 0, 0);
        public static float airDensity = 0.01f;
        public AnimationCurve airDensityCurve = new AnimationCurve();
        public AnimationCurve liftCoeff = new AnimationCurve();
        public AnimationCurve dragCoeff = new AnimationCurve();
        public AnimationCurve torqueCoeff = new AnimationCurve();
        public AnimationCurve inducedDragCurve = new AnimationCurve();

        public float getAirDensity(float height)
        {
            return airDensity * airDensityCurve.Evaluate(height);
        }

        public Vector3 CalcucateCoefficients(float angleOfAttack)
        {
            float liftC = liftCoeff.Evaluate(angleOfAttack);
            float dragC = dragCoeff.Evaluate(angleOfAttack);
            float torqueC = torqueCoeff.Evaluate(angleOfAttack);
            return new Vector3(liftC, dragC, torqueC);
        }
    }
}
