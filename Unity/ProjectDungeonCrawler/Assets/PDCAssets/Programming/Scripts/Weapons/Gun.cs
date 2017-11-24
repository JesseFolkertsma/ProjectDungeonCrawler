using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : Weapon {
    public Transform gunEnd;

    public MuzzleFlash muzzleFlash;

    float timer = 0;

    public override bool Attack()
    {
        if (data.currentAmmo > 0)
        {
            if (timer > Time.time)
            {
                return true;
            }
            else
            {
                timer = Time.time + 1 / data.attackRate;
                return base.Attack();
            }
        }
            else return false;
    }

    public void MuzzleFlash()
    {
        data.currentAmmo--;
        PlayVisuals();
        pc.DoAttackEffect();
        GeneralCanvas.canvas.SetAmmoCount(true, true, data.maxAmmo, data.currentAmmo);
    }

    public override void PlayVisuals()
    {
        base.PlayVisuals();
        muzzleFlash.Play();
    }
}
