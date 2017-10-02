using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkObjectSpawner : NetworkBehaviour {

    public GameObject objectToSpawn;

    public override void OnStartServer()
    {
        if(objectToSpawn != null)
        {
            GameObject go = (GameObject)Instantiate(objectToSpawn, transform.position, transform.rotation);
            NetworkServer.Spawn(go);
        }
    }
}
