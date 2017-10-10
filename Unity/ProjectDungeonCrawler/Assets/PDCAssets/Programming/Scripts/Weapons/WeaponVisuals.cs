using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisuals : MonoBehaviour {
    public Transform gunEnd;

    public GameObject muzzleFlash;

    public void ShootVisuals()
    {
        Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
    }
}
