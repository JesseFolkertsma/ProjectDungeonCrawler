using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : Weapon {
    public Transform gunEnd;
    public GameObject muzzleFlash;

    public override void Attack()
    {
        base.Attack();
    }

    public void MuzzleFlash()
    {
        //pc.AttackEffect();
    }

    public override void PlayVisuals()
    {
        base.PlayVisuals();
        Attack();
        //muzzleFlash.Play();
        Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
    }
}
