using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticle : MonoBehaviour {
    ParticleSystem[] particles;
    Animation[] animations;
    AudioSource aSource;

    private void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        aSource = GetComponentInChildren<AudioSource>();
        animations = GetComponentsInChildren<Animation>();
    }

    public void Activate()
    {
        if(aSource != null)
            aSource.Play();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
        foreach (Animation a in animations)
        {
            a.Play();
        }
    }
}
