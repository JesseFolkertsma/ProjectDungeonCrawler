using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponVisuals : NetworkBehaviour {
    public Transform gunEnd;

    public GameObject muzzleFlash;
    public Animator anim;

    [Command]
    public void CmdShootVisuals()
    {
        anim.SetTrigger("Shoot");
        GameManager.instance.SpawnObjectOnServer(muzzleFlash, gunEnd);
    }
}
