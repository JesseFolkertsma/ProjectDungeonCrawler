using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Player : NetworkBehaviour {
    //Public variables
    public PlayerData data;
    public GameObject playerPrefab;

    //Live instances
    public PlayerPrefabData instance_PlayerPrefab;

    //Delagates
    public delegate void OnGameSceneEnter();
    public OnGameSceneEnter onGameSceneEnter;
    public delegate void OnMapSceneEnter();
    public OnMapSceneEnter onMapSceneEnter;

    //Shortcuts
    public NWPlayerCombat pCombat
    {
        get
        {
            return instance_PlayerPrefab.combat;
        }
    }

    public NetworkedController pControls
    {
        get
        {
            return instance_PlayerPrefab.controls;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string id = GetComponent<NetworkIdentity>().netId.ToString();
        Player pc = GetComponent<Player>();
        PlayerManager.RegisterPlayer(id, pc);
    }

    #region GameSceneEnter
    public void GameSceneEnter()
    {
        CmdGameSceneEnter();
    }

    [Command]
    void CmdGameSceneEnter()
    {
        RpcGameSceneEnter();
    }

    [ClientRpc]
    void RpcGameSceneEnter()
    {
        instance_PlayerPrefab = Instantiate(playerPrefab, transform.position, transform.rotation, transform).GetComponent<PlayerPrefabData>();

        if (!isLocalPlayer)
            return;

        onGameSceneEnter();
    }
    #endregion

    public void MapSceneEnter()
    {
        onMapSceneEnter();
    }

    //Chat functions

    //Popup functions
}
