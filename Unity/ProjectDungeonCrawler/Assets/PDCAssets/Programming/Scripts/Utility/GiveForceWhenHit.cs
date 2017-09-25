using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using System;

[RequireComponent(typeof(Rigidbody))]
public class GiveForceWhenHit : MonoBehaviour, IHitable
{
    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
    {
        Vector3 direction = (transform.position - shotPosition).normalized;
        GetComponent<Rigidbody>().AddForce(direction * 250 * 20);
    }

    public Vector3 ObjectCenter
    {
        get
        {
            return transform.position;
        }
    }
}
