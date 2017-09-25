using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using System;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSoundOnHit : MonoBehaviour,IHitable {
    public Vector3 ObjectCenter
    {
        get
        {
            return transform.position;
        }
    }

    public AudioClip[] clips;
    AudioSource audioS;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
    }

    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
    {
        print(123);
        if(clips.Length > 0)
        {
            audioS.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            audioS.Play();
        }
    }
}
