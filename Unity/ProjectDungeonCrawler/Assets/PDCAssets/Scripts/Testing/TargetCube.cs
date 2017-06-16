using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using System;

public class TargetCube : MonoBehaviour, IHitable
{
    float hp = 100;
    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
    {
        hp -= damage;
        if(hp < 1)
            Destroy(gameObject);
    }

    public Vector3 ObjectCenter
    {
        get
        {
            return transform.position;
        }
    }
}
