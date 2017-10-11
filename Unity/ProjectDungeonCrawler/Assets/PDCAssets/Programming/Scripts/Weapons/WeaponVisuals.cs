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
        GameManager.instance.SpawnObjectOnServer(muzzleFlash, gunEnd);
    }
}
