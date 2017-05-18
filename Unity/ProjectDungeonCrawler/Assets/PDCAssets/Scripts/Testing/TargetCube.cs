using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using System;

public class TargetCube : MonoBehaviour, IHitable
{
    public void GetHit(float damage, EffectType hitType)
    {
        Destroy(gameObject);
    }
}
