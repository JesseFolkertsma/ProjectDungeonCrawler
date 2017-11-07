using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    const string REMOTE_PLAYER_LAYER = "RemotePlayer";
    
    //Public variables
    public NetworkedController controls;
    public NWPlayerCombat combat;
    public GameObject[] goToDisable;
    public Behaviour[] compToDisable;

    //Private variables

    //Private serializable variables
    [SerializeField]
    Camera mainCamera;

    public void Start()
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
            Debug.Log("INIT COMBAT");
            combat.Init();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string id = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager.RegisterPlayer(id, GetComponent<NWPlayerCombat>());
    }

    private void OnDisable()
    {
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        PlayerManager.RemovePlayer(gameObject.name);
    }

    void DisableGameObjects()
    {
        foreach (GameObject go in goToDisable)
        {
            go.SetActive(false);
        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < compToDisable.Length; i++)
        {
            Debug.Log("Is a me: " + gameObject.name + " yes and i will disable component: " + compToDisable[i].GetType());
            compToDisable[i].enabled = false;
        }
    }

    void SetAsRemotePlayer()
    {
        StaticFunctions.SetLayerRecursively(transform, REMOTE_PLAYER_LAYER);
    }
}
