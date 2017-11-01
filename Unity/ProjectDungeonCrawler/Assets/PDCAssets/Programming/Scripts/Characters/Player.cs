using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Player : NetworkBehaviour {

    public enum GameState
    {
        Lobby,
        Gameplay,
        Map,
    };

    //Public variables
    public PlayerData data;
    public GameObject playerPrefab;
    public GameState gameState;

    //Live instances
    public PlayerPrefabData instance_PlayerPrefab;
    public GameplayPrefabData gameplay_PlayerPrefab
    {
        get
        {
            if (gameState == GameState.Gameplay)
            {
                GameplayPrefabData data = (GameplayPrefabData)instance_PlayerPrefab;
                return data;
            }
            else return null;
        }
    }

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
            if (gameState == GameState.Gameplay)
                return gameplay_PlayerPrefab.combat;
            else return null;
        }
    }
    public NetworkedController pControls
    {
        get
        {
            if (gameState == GameState.Gameplay)
                return gameplay_PlayerPrefab.controls;
            else return null;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string id = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager.RegisterPlayer(id, this);
        gameState = GameState.Gameplay;
        Debug.Log("Am i local? answser: " + isLocalPlayer.ToString());
        GameSceneEnter();
    }

    private void OnDisable()
    {
        PlayerManager.RemovePlayer(gameObject.name);
    }

    #region GameSceneEnter
    public void GameSceneEnter()
    {

    }
    #endregion

    public void MapSceneEnter()
    {
        onMapSceneEnter();
    }

    [Command]
    void CmdSetName(string name)
    {
        RpcSetName(name);
    }

    [ClientRpc]
    void RpcSetName(string name)
    {
        data.playerName = name;
        Debug.Log("ID: " + data.playerID + ", Name: " + data.playerName);
    }

    //Chat functions

    public void SendChatMessage(string message)
    {
        CmdSendMessage(data.playerName + ": " + message);
    }

    [Command]
    void CmdSendMessage(string message)
    {
        foreach (KeyValuePair<string, Player> kvp in PlayerManager.PlayerList())
        {
            kvp.Value.TargetRecieveMessage(kvp.Value.connectionToClient, message);
        }
    }

    [TargetRpc]
    void TargetRecieveMessage(NetworkConnection conn, string message)
    {
        //hud.SendMessage(message);
    }

    //Popup functions

    [Command]
    void CmdSendPopup(string message, string sender, bool dontSendToSender)
    {
        foreach (KeyValuePair<string, Player> kvp in PlayerManager.PlayerList())
        {
            if (kvp.Key == sender && dontSendToSender) continue;
            kvp.Value.TargetReceivePopup(kvp.Value.connectionToClient, message);
        }
    }

    [TargetRpc]
    void TargetReceivePopup(NetworkConnection conn, string message)
    {
        //hud.FeedMessage(message);
    }
}
