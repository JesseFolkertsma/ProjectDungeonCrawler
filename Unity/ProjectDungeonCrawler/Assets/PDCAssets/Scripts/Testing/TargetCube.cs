using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using System;

public class TargetCube : MonoBehaviour, IHitable
{
    public void GetHit(float damage, EffectType hitType, StatusEffect[] effects)
    {
        Destroy(gameObject);
    }
}
