using UnityEngine;
using Unity.FPS.AI;

namespace Engine.Props
{
    public class RobotProps : MonoBehaviour
    {
        public PatrolPath PatrolPath { get; set; }

        [Tooltip("The random hit damage effects")]
        public ParticleSystem[] RandomHitSparks;

        public ParticleSystem[] OnDetectVfx;
        public AudioClip OnDetectSfx;

        [Header("Sound")] public AudioClip MovementSound;
        // Start is called before the first frame update
    }
}