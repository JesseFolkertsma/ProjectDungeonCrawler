using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkedController))]
public class PlayerInteraction : NetworkBehaviour {
    [SerializeField]
    Camera playerCam;

    public float interactRange = 3;
    public float pushForce = 300;
    public LayerMask remoteplayerLayer;
    public LayerMask pickupLayer;

    private void Update()
    {
    }

    void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactRange))
        {
            if (hit.transform.tag == "Button")
            {
                CmdGetObjectAuth(hit.transform.GetComponent<NetworkIdentity>());
                hit.transform.GetComponent<SpawnButton>().CmdSpawn();
            }
        }
    }

    [Command]
    public void CmdGetObjectAuth(NetworkIdentity objectID)
    {
        var otherOwner = objectID.clientAuthorityOwner;
        NetworkIdentity myID = GetComponent<NetworkIdentity>();

        if (otherOwner == myID.connectionToClient)
        {
            return;
        }
        else
        {
            if (otherOwner != null)
            {
                objectID.RemoveClientAuthority(otherOwner);
            }
            objectID.AssignClientAuthority(myID.connectionToClient);
        }
    }
}
