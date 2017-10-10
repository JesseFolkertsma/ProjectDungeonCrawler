using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Hosting : MonoBehaviour {
    public string nickName;

    public string roomName;

    public Transform LobbyMenu;

    private NetworkManager networkManager;

    public void Start() {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }
    public void HostGame() {
        if (roomName != "") {
            networkManager.matchMaker.CreateMatch(roomName, 4, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            print("Creating a room named: " + roomName);
            LobbyMenu.GetComponent<LobbyManager>().StartLobby(nickName);
        }
        else {
        }
    }
    public void ChangeRoomName(string name) {
        roomName = name;
    }
    public void Nickname(string _nickName) {
        nickName = _nickName;
    }
}
