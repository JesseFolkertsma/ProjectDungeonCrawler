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

    public string nickName;

    public void Start() {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }
    public void Error(int i) {
        error.text = errorMessages[i];
    }


    // Joining section //

    

}


