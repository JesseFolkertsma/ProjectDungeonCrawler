using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Joining : MonoBehaviour {
    public Text statusText;

    public List<GameObject> lobbyList = new List<GameObject>();

    public Transform contentLobbyList;

    public GameObject lobbyPref;

    private NetworkManager networkManager;

    public void Start() {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null) {
            MatchMakerOn();
        }
    }
    public void MatchMakerOn() {
        networkManager.StartMatchMaker();
    }
    public void RefreshLobbyList() {
        if(networkManager.matchMaker == null) {
            MatchMakerOn();
        }
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        statusText.text = "Loading...";
    }

    public void OnMatchList(bool succes, string extendedInfo, List<MatchInfoSnapshot> matches) {
        statusText.text = "";
        ClearLobbyList();
        if (matches.Count == 0) {
            statusText.text = "No lobbies available";
            print("No lobbies available");
            return;
        }
        foreach (MatchInfoSnapshot i in matches) {
            print("Found a match with the name:" + i.name);
            GameObject _newLobby = Instantiate(lobbyPref);
            _newLobby.transform.SetParent(contentLobbyList, false);
            _newLobby.GetComponent<LobbyItemHelper>().Setup(i, JoinLobby);
            lobbyList.Add(_newLobby);
        }
    }
    public void ClearLobbyList() {
        print("Clearing Lobby List");
        for (int i = 0; i < lobbyList.Count; i++) {
            Destroy(lobbyList[i]);
        }
        lobbyList.Clear();
    }
    public void JoinLobby(MatchInfoSnapshot _match) {
        Debug.Log("Joining :" + _match.name);
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearLobbyList();
        statusText.text = "Joining...";
    }

}
