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

    private void Awake()
    {
        joint = GetComponent<CharacterJoint>();
    }

    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
    {
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
