using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPrefabData : NetworkBehaviour {

    const string REMOTE_PLAYER_LAYER = "RemotePlayer";

    //Public variables
    public NetworkedController controls;
    public NWPlayerCombat combat;

    //Private variables

    //Private serializable variables
    [SerializeField]
    GameObject[] goToDisable;
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
            DisableGameObjects();
            mainCamera = Camera.main;
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }

        combat = GetComponent<NWPlayerCombat>();
        controls = GetComponent<NetworkedController>();
        if (combat != null)
        {
            combat.Setup();
        }
        GetComponent<Player>().GameSceneEnter();
    }

    void DisableGameObjects()
    {
        foreach(GameObject go in goToDisable)
        {
            go.SetActive(false);
        }
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

        PlayerManager.RemovePlayer(gameObject.name);
    }
}
