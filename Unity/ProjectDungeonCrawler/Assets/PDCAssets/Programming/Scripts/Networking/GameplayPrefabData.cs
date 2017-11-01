using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameplayPrefabData : PlayerPrefabData {

    //Public variables
    public NetworkedController controls;
    public NWPlayerCombat combat;

    //Private variables

    //Private serializable variables
    [SerializeField]
    Camera mainCamera;

    public override void Init(bool isLocal)
    {
        base.Init(isLocal);
        if (!isLocal)
        {

        }
        else
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }

        combat = GetComponent<NWPlayerCombat>();
        controls = GetComponent<NetworkedController>();
        if (combat != null)
        {
            Debug.Log("INIT COMBAT");
            combat.Init(isLocal);
        }
    }

    private void OnDisable()
    {
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);
    }
}
