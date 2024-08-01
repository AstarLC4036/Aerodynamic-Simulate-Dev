using UnityEngine;

namespace APG.Noise
{
    public class SimpleNoiseFilter : INoiseFilter
    {
        NoiseSettings.SimpleNoiseSettings settings;
        Noise noise = new Noise();

        public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
        {
            this.settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            //float noiseValue = (noise.Evaluate(point * settings.roughness + settings.center) + 1) * 0.5f;
            float noiseValue = 0;
            float frequency = settings.baseRoughness;
            float amplitude = 1;

            for (int i = 0; i < settings.layers; i++)
            {
                float v = noise.Evaluate(point * frequency + settings.center);
                noiseValue += (v + 1) * 0.5f * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            //noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
            noiseValue = noiseValue - settings.minValue;

            return noiseValue * settings.strength;
        }
    }
}
