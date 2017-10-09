using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Destroyable : NetworkBehaviour, IHitable
{
    public NetworkConnection networkConn
    {
        get
        {
            return null;
        }
    }

    public NetworkInstanceId networkID
    {
        get
        {
            return netId;
        }
    }

    public string objectName
    {
        get
        {
            return gameObject.name;
        }
    }

    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        Debug.Log("FUCK MAN U HIT ME MAH NEM IS: " + gameObject.name + " HE DID " + dmgPck.ToString() + " DAMAGE!");
    }
}
