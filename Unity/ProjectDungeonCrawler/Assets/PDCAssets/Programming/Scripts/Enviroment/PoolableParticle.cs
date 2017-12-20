using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticle : MonoBehaviour {
    public ParticleSystem[] particles;
    public AudioSource aSource;

    private void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        aSource = GetComponentInChildren<AudioSource>();
    }

    public void Activate()
    {
        if(aSource != null)
            aSource.Play();
        foreach(ParticleSystem p in particles)
        {
            p.Play();
        }
    }
}
