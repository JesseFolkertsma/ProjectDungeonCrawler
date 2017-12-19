using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticle : MonoBehaviour {
    public ParticleSystem[] particles;

    private void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void Activate()
    {
        foreach(ParticleSystem p in particles)
        {
            p.Play();
        }
    }
}
