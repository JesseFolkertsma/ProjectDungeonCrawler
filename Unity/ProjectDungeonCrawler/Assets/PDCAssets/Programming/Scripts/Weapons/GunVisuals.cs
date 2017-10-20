using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunVisuals : WeaponVisuals {
    public Transform gunEnd;

    public MuzzleFlash muzzleFlash;

    public override void Attack()
    {
        if (!IsInBaseState())
            return;

        base.Attack();
    }

    public void MuzzleFlash()
    {
        muzzleFlash.Play();
        pc.DoAttackEffect();
    }

    public override void RemotePlayerEffectVisuals()
    {
        muzzleFlash.Play();
    }

    public override void RemotePlayerStartVisuals()
    {
    }
}
