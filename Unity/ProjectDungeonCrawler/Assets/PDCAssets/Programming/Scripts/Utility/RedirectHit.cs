using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RedirectHit : MonoBehaviour, IHitable {

    public GameObject objectToDirectTo;

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

    public string objectName
    {
        get
        {
            return objectToDirectTo.name;
        }
    }

    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        iHit.RpcGetHit(dmgPck);
    }
}
