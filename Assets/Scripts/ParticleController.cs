using UnityEngine;

namespace CelesteGameFeel
{
    public class ParticleController : MonoBehaviour
    {
        [Header("Particles")]
        [SerializeField] private ParticleSystem dashTrailParticles;
        [SerializeField] private ParticleSystem dashSpreadParticles;
        [SerializeField] private ParticleSystem groundDustParticles;

        public void PlayDashParticles()
        {
            dashSpreadParticles.Play();
            dashTrailParticles.Play();
        }

        public void StopDashParticles()
        {
            dashSpreadParticles.Stop();
            dashTrailParticles.Stop();
        }

        public void PlayGroundParticles()
        {
            groundDustParticles.Play();
        }
    }
}