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

    public NetworkConnection networkConn
    {
        get
        {
            return iHit.networkConn;
        }
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

    public void GetHit(NetworkPackages.DamagePackage dmgPck)
    {
        iHit.GetHit(dmgPck);
    }
}
