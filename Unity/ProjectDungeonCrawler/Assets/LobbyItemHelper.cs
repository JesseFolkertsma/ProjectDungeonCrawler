using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyItemHelper : MonoBehaviour {
    [SerializeField]
    Text lobbyName;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match) {
        match = _match;

        lobbyName.text = match.name;
    }
}
