using UnityEngine;

namespace APG.Noise
{
    public class NoiseFilterFactory
    {
        public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
        {
            switch(settings.filterType)
            {
                case NoiseSettings.FilterType.Simple:
                    return new SimpleNoiseFilter(settings.simpleNoiseSettings);
                case NoiseSettings.FilterType.Ridgid:
                    return new RidgidNoiseFilter(settings.ridgidNoiseSettings);
            }

            return null;
        }
    }
}
