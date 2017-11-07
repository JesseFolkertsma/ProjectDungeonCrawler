using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkedUI : NetworkBehaviour {

    GeneralCanvas GC {
        get {
            return GeneralCanvas.canvas;
        }
    }
    NWPlayerCombat pc;
    public void Init(NWPlayerCombat _pc){
        CmdFeedMessage(gameObject.name + " has joined the lobby!");
        pc = _pc;

    }
    #region Feed/Notifications
    [Command]
    public void CmdFeedMessage(string message) {
        RpcFeedMessage(message);
    }
    [ClientRpc]
    public void RpcFeedMessage(string message) {
        GC.FeedMessage(message);
    }
    #endregion
    #region Chat
    //Variables//
    [Command]
    public void CmdChatMessage(string message) {
        RpcChatMessage(message);
    }
    [ClientRpc]
    public void RpcChatMessage(string message) {
        GC.SendMessage(message);
    }
    public void FieldEndEdit() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            CmdChatMessage(pc.objectName + ": " + GC.inputField.text);
            GC.inputField.text = "";
            GC.ToggleChat();
        }
    }
    #endregion

}
