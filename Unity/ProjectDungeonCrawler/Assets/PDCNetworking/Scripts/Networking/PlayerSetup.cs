using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    const string REMOTE_PLAYER_LAYER = "RemotePlayer";

    [SerializeField]
    Behaviour[] compToDisable;

    [SerializeField]
    Camera mainCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            SetAsRemotePlayer();
        }
        else
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string id = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerController pc = GetComponent<PlayerController>();
        GameManager.RegisterPlayer(id, pc);
    }

    void DisableComponents()
    {
        for (int i = 0; i < compToDisable.Length; i++)
        {
            compToDisable[i].enabled = false;
        }
    }

    void SetAsRemotePlayer()
    {
        SetLayerRecursively(transform, REMOTE_PLAYER_LAYER);
    }

    void SetLayerRecursively(Transform objectToSet, string layerName)
    {
        objectToSet.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform t in objectToSet)
        {
            SetLayerRecursively(t, layerName);
        }
    }

    private void OnDisable()
    {
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        GameManager.RemovePlayer(transform.name);
    }
}
