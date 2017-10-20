using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameSceneEnter(Player _oldPlayer, GameObject _newPlayer)
    {
        var conn = _oldPlayer.connectionToClient;
        var newPlayer = Instantiate<GameObject>(_newPlayer);

        NetworkServer.ReplacePlayerForConnection(conn, newPlayer, _oldPlayer.playerControllerId);
    }
}