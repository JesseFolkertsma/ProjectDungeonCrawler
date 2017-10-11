using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponVisuals : NetworkBehaviour {
    public Transform gunEnd;

    public GameObject muzzleFlash;
    public Animator anim;
    
    public void ShootVisuals()
    {
        anim.SetTrigger("Shoot");
        Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
    }
}
