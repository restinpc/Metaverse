using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProps : MonoBehaviour
{
    [Tooltip("The random hit damage effects")]
    public ParticleSystem[] RandomHitSparks;

    public ParticleSystem[] OnDetectVfx;
    public AudioClip OnDetectSfx;
}
