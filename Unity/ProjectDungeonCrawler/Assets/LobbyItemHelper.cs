using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyItemHelper : MonoBehaviour {
    [SerializeField]
    Text lobbyName;

    public delegate void JoinLobbyDelegate(MatchInfoSnapshot match);
    public JoinLobbyDelegate joinLobbyCallBack;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinLobbyDelegate _joinRoomCallBack) {
        match = _match;
        joinLobbyCallBack = _joinRoomCallBack;
        lobbyName.text = match.name;
    }
    public void JoinLobby() {
        joinLobbyCallBack.Invoke(match);
    }
}
