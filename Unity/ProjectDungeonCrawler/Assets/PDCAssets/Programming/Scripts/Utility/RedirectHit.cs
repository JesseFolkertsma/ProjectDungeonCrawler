using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RedirectHit : MonoBehaviour, IHitable {

    public GameObject objectToDirectTo;
    public float multiplier = 1f;

    IHitable iHit;

    private void Awake()
    {
        iHit = objectToDirectTo.GetComponent<IHitable>();
    }

    public NetworkInstanceId networkID
    {
        get
        {
            return iHit.networkID;
        }
    }

    public string objectID
    {
        get
        {
            return objectToDirectTo.name;
        }
    }

    public string objectName
    {
        get
        {
            return iHit.objectName;
        }
    }

    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        Debug.LogError("I WILL REDIRECT");
        Debug.Log(gameObject.name + ": i have " + dmgPck.damage.ToString() + " damage times " + multiplier.ToString() + " equals " + (dmgPck.damage * multiplier).ToString());
        dmgPck.damage *= multiplier;
        //iHit.RpcGetHit(dmgPck);
    }
}
