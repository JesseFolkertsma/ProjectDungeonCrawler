using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : Weapon {
    public Transform gunEnd;

    public MuzzleFlash muzzleFlash;

    public override void Attack()
    {
        base.Attack();
    }

    public void MuzzleFlash()
    {
        pc.DoAttackEffect();
    }

    public override void PlayVisuals()
    {
        base.PlayVisuals();
        muzzleFlash.Play();
    }
}
