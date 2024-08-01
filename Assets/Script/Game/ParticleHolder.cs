using System.Collections;
using UnityEngine;

namespace Assets.Script.Game
{
    public class ParticleHolder : MonoBehaviour
    {
        public ParticleSystem[] particleSystems;

        private bool isParticlePlaying = false;

        public bool isPlaying
        {
            get
            {
                return isParticlePlaying;
            }
        }

        public void PlayParticle()
        {
            foreach (ParticleSystem particle in particleSystems)
            {
                particle.Play();
            }
            isParticlePlaying = true;
        }

        public void StopParticle()
        {
            foreach (ParticleSystem particle in particleSystems)
            {
                particle.Stop();
            }
            isParticlePlaying = false;
        }
    }
}