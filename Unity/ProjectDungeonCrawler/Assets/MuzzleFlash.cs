using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {

    ParticleSystem ps;
    AudioSource aSource;

    private void Awake()
    {
        //ps = GetComponent<ParticleSystem>();
        aSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        //ps.Play();
        aSource.Play();
    }
}
