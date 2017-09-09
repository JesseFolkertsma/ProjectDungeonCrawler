using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using System;

[RequireComponent(typeof(CharacterJoint))]
public class DetachableLimb : MonoBehaviour, IHitable{

    bool isDetached = false;
    CharacterJoint joint;

    public Vector3 ObjectCenter
    {
        get
        {
            return transform.position;
        }
    }

    private void Awake()
    {
        joint = GetComponent<CharacterJoint>();
    }

    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
    {
        GameManager.SoundObj obj = new GameManager.SoundObj();
        obj.clip = GameManager.instance.boneHit;
        obj.volume = 100;
        GameManager.instance.SpawnSound(obj, transform.position);
        if (!isDetached)
        {
            isDetached = true;
            transform.parent = null;
            joint.breakForce = 0;
        }
        Vector3 direction = (transform.position - shotPosition).normalized;
        GetComponent<Rigidbody>().AddForce(direction * 250 * 20);
    }
}
