using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager thisManager;

    public Transform playerContent;
    public GameObject playerPref;
    public List<GameObject> players = new List<GameObject>();

    public bool lobby;

    NetworkManager networkManager;

    public void Update() {
        if (lobby) {
            if (Input.GetButtonDown("Cancel")) {
                lobby = false;
                LeaveLobby();
            }
        }
    }
    public void Start() {
        networkManager = NetworkManager.singleton;
        lobby = false;
    }
    public void StartLobby(string name) {
        lobby = true;
        PlayerAdd(name);
    }
    public void PlayerAdd(string name) {
        GameObject _newPlayer = Instantiate(playerPref);
        _newPlayer.transform.SetParent(playerContent, false);
        _newPlayer.transform.GetChild(0).GetComponent<Text>().text = name;
        players.Add(_newPlayer);
    }
    public void PlayerRemove(string name) {
        foreach(GameObject player in players) {
            if(player.transform.GetChild(0).GetComponent<Text>().text == name) {
                Destroy(player);
                players.Remove(null);
            }
        }
    }
    public void PlayerClear() {
        foreach(GameObject player in players) {
            Destroy(player);
        }
        players.Clear();
    }
    public void LeaveLobby() {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
        PlayerClear();
        Debug.Log("Closed Lobby");
        
    }

}
