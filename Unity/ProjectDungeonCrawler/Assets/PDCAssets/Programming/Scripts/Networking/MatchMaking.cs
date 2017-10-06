using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMaking : MonoBehaviour {
    public InputField name;
    public InputField ip;
    public Text error;

    private NetworkManager networkManager;

    public string[] errorMessages;

    public void Start() {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }
    public void JoinGame() {
        if(name.text != "" || ip.text != "") {
            Error(2);
        }
        else {
            Error(0);
        }
    }
    public void HostGame() {
        if(name.text != "") {
            //networkManager.matchMaker.CreateMatch("hary", 4, true, "", networkManager.OnMatchCreate);
        }
        else {
            Error(1);
        }
    }
    public void Error(int i) {
        error.text = errorMessages[i];
    }
}
