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
        Debug.Log("I am ze button and i am pressed c:");
        GameObject go = (GameObject)Instantiate(objectToSpawn, spawnPosition.position, spawnPosition.rotation);
        NetworkServer.Spawn(go);
    }
}
