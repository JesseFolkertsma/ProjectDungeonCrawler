using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnButton : NetworkBehaviour {
    public GameObject objectToSpawn;
    public Transform spawnPosition;
    
    [Command]
    public void CmdSpawn()
    {
        GameObject go = (GameObject)Instantiate(objectToSpawn, spawnPosition.position, spawnPosition.rotation);
        NetworkServer.Spawn(go);
    }
}
