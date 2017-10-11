using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class NetworkedGunFX : NetworkBehaviour {

    public Transform gunEnd;
    public GameObject muzzleflash;
    public Animator anim;

    public void Setup()
    {

    }

    [ClientRpc]
    public void RpcVisuals(string name)
    {
        if (name == gameObject.name)
        {
            anim.SetTrigger("Shoot");
            GameManager.instance.SpawnObjectOnServer(muzzleflash, gunEnd);
        }
    }
    
}
