using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMaking : MonoBehaviour {


    private MyNetworkManager networkManager;

    public string[] errorMessages;

    // General //

    public Text error;

    public string nickName;

    public void Start() {
        networkManager = MyNetworkManager.mySingleton;
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }
    public void Error(int i) {
        error.text = errorMessages[i];
    }


    // Joining section //

    

}


