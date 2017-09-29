using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;


public class RPSMenu : NetworkBehaviour {
    public int choice;
    public int currentMatchID;
    [ClientRpc]
    public void RpcRequest(int matchID) {
        currentMatchID = matchID;
        //Activate popup for rps request
    }
    [ClientRpc]
    public void RpcToggle(int i) {
        //Can Toggle the UI off and On
    }
    [Command]
    public void CmdRequestAnswer(int i) {
        if(i == 0) {
            // Call RPSMatch in RPSManager and tell it that you accepted
        }
        else {
            // Call RPSMatch in RPSManager and tell it that you declined
        }
    }
    public void Result(string playerID){
        if(playerID == transform.name) {
            //chicken dinner bois!
        }
        else {
            //good luck next time
        }
    }
    public void Select(int i) {
        choice = i;
    }
    [Command]
    public void CmdLockIn() {
        // Call RPSMatch Answer(choice, transform.name);
    }
}
