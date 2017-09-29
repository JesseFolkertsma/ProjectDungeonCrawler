using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemPickup : NetworkBehaviour {

    public Item itemData;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.root.GetComponent<Inventory>().Add(itemData);
        CmdDestroy();
    }

    [Command]
    void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
