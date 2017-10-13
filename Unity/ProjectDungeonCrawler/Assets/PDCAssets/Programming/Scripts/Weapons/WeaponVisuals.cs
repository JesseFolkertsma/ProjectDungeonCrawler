using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponVisuals : MonoBehaviour {
    public Transform gunEnd;

    public MuzzleFlash muzzleFlash;
    public Animator anim;
    
    public void ShootVisuals()
    {
        anim.SetTrigger("Shoot");
        muzzleFlash.Play();
    }
}
