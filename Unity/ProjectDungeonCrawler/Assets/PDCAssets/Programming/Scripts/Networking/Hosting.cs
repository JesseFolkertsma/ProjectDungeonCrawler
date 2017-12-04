using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Hosting : MonoBehaviour {
    public string roomName;

    public Transform LobbyMenu;

    private MyNetworkManager networkManager;

    public void Start() {
        networkManager = MyNetworkManager.mySingleton;
        if (networkManager.matchMaker == null) {
            MatchMakerOn();
        }
    }
    public void MatchMakerOn() {
        networkManager.StartMatchMaker();
    }
    public void Update() {
        if (Input.GetButtonDown("Jump")) {
            networkManager.StartMatchMaker();
        }
    }
    public void HostGame() {
        if(networkManager.matchMaker == null) {
            MatchMakerOn();
        }
        if (roomName != "") {
            networkManager.matchMaker.CreateMatch(roomName, 4, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
            print("Creating a room named: " + roomName);
        }
        else {
            print("An error occured hosting a game : No room name put in");
        }
    }
    public void ChangeRoomName(string name) {
        roomName = name;
    }
    public void Nickname(string _nickName) {
        PlayerInfo.instance.playerName = _nickName;
    }
}
