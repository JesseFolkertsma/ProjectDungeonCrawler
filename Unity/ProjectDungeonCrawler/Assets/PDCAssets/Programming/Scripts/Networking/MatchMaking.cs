using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMaking : MonoBehaviour {


    private NetworkManager networkManager;

    public string[] errorMessages;




    // General //

    public Text error;

    public void Start() {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }
    public void Error(int i) {
        error.text = errorMessages[i];
    }

    // Joining section //

    public Text statusText;

    public List<GameObject> lobbyList = new List<GameObject>();

    public Transform contentLobbyList;

    public GameObject lobbyPref;

    public void JoinGame() {

    }

    public void RefreshLobbyList() {
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        statusText.text = "Loading...";
        
    }

    public void OnMatchList(bool succes, string extendedInfo, List<MatchInfoSnapshot> matches) {
        statusText.text = "";

        if(succes){
            statusText.text = "No lobbies available";
            return;
        }
        ClearLobbyList();
        foreach (MatchInfoSnapshot i in matches) {
            GameObject _newLobby = Instantiate(lobbyPref, Vector3.zero, Quaternion.identity);
            _newLobby.transform.SetParent(contentLobbyList);
            //Call LobbyItemHelper class and give lobby name
            //

        }
    }
    public void ClearLobbyList() {
        for(int i = 0; i < lobbyList.Count; i++) {
            Destroy(lobbyList[i]);
        }
        lobbyList.Clear();
    }

    // Hosting section //

    public string roomName;


    public void HostGame() {
        if (roomName != "") {
            networkManager.matchMaker.CreateMatch("hary", 4, true, "", "", "", 0 ,0 , networkManager.OnMatchCreate);
            print("Creating a room named: " + roomName);
        }
        else {
        }
    }
    public void ChangeRoomName(string name) {
        roomName = name;
    }
}


