using System;
using UnityEngine;
using static BA.Utility.SimpleAnimator;

namespace BA.Utility
{
    [Serializable]
    public class SimpleAnimation
    {
        public Transform transform;

        public AnimationType animationType = AnimationType.None;

        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
    }
}
