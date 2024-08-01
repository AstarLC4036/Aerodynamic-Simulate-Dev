using UnityEngine;

namespace APG.Noise
{
    public interface INoiseFilter
    {
        float Evaluate(Vector3 point);
    }
}
