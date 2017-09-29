using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSMatch : MonoBehaviour {
    public RPSMenu challenger;
    public RPSMenu challenged;
    enum RPSOptions {Empty, Rock, Paper, Scissors };
    RPSOptions challengerChoice;
    RPSOptions challengedChoice;
    bool challengerReady;
    bool challengedReady;

    int matchId;

    public RPSMatch(RPSMenu _challenger, RPSMenu _challenged, int _matchId) {
        challenger = _challenger;
        challenged = _challenged;
        matchId = _matchId;
    }
    public void StartRPS() {
        challenged.RpcRequest(matchId);
        challengerReady = true;
    }
    public void ResponseRequest(int i) {
        if(i == 0) {
            // start picking phase
            challengerReady = false;
            challengedReady = false;        }
        else {
            // cancel this match
        }
    }
    public void Answer(int i, string playerID) {
        if(playerID == challenged.transform.name) {
            challengedChoice = (RPSOptions)i;
            challengedReady = true;
        }
        else if(playerID == challenger.transform.name) {
            challengerChoice = (RPSOptions)i;
            challengerReady = true;
        }
        if(challengerReady == true && challengedReady == true) {
            EndMatch();
                
        }

    }
    void EndMatch() {
        //check who won and activate 
    }
}
