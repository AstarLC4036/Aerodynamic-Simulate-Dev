using BA.Utility;
using UnityEngine;

namespace APG.Noise
{
    [System.Serializable]
    public class NoiseSettings
    {
        public enum FilterType
        {
            Simple,
            Ridgid
        };

        public FilterType filterType;

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RidgidNoiseSettings ridgidNoiseSettings;

        [System.Serializable]
        public class SimpleNoiseSettings
        {
            [Range(1, 10)]
            public int layers = 1;
            public float strength = 1;
            public float baseRoughness = 1;
            public float roughness = 2;
            public float persistence = 0.5f;
            public float minValue = 1;
            public Vector3 center;
        }

        [System.Serializable]
        public class RidgidNoiseSettings : SimpleNoiseSettings
        {
            [Header("Special Settings")]
            public float weightMultiplier = 0.8f;
        }
    }
}
