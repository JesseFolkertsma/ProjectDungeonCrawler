using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisuals : MonoBehaviour {
    public Transform gunEnd;

    public GameObject muzzleFlash;
    public Animator anim;

    public void ShootVisuals()
    {
        anim.SetTrigger("Shoot");
        //Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
    }
}
