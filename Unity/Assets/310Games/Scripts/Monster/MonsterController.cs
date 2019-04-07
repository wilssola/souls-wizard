using UnityEngine;

namespace TecWolf.Monster
{
    public class MonsterController : MonoBehaviour
    {
        public ParticleSystem Particle;
        public bool First;

        private void Start()
        {
            Particle.transform.name = "Magic";

            Particle.transform.parent = transform.parent.transform.parent;
        }

        private void OnEnable()
        {
            Particle.Play();
        }

        private void OnDisable()
        {
            if (!First)
            {
                Particle.Play();
            }
        }

        private void OnDestroy()
        {
            if (Particle != null)
            {
                Particle.Play();

                Destroy(Particle, 1.5f);
            }
        }
    }
}
