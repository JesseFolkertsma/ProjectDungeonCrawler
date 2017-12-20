using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyableObject : NetworkBehaviour, IHitable {
    public GameObject replacePref;
    public bool hary;

    public string objectID {
        get {
            return "Breakable";
        }
    }

    public string objectName {
        get {
            return gameObject.name;
        }
    }

    public NetworkInstanceId networkID {
        get {
            return netId;
        }
    }

    public override bool InvokeRPC(int cmdHash, NetworkReader reader) {
        return base.InvokeRPC(cmdHash, reader);
    }

    public void Update() {
        if (hary) {
            Replace();
            hary = false;
        }
    }
    public void Replace() {
        GameObject newObject = Instantiate(replacePref, transform);
        newObject.transform.SetParent(null, true);
        //newObject.transform.position = transform.TransformPoint(transform.position);
        Destroy(gameObject);
    }

    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck) {
        throw new System.NotImplementedException();
    }
}
