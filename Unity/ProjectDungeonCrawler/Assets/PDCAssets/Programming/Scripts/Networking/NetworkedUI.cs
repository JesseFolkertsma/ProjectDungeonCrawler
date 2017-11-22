using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkedUI : NetworkBehaviour {
    #region General
    GeneralCanvas GC {
        get {
            return GeneralCanvas.canvas;
        }
    }
    NWPlayerCombat pc;

    private void Update()
    { 
        if (!isLocalPlayer)
            return;
        FieldEndEdit();
    }

    public void Init(NWPlayerCombat _pc){
        if (!isLocalPlayer)
            return;
        CmdFeedMessage(_pc.objectName + " has joined the lobby!");
        pc = _pc;
    }
#endregion
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
            if (!string.IsNullOrEmpty(GC.inputField.text) && !GC.inputField.isFocused)
            {
                CmdChatMessage(pc.objectName + ": " + GC.inputField.text);
                GC.inputField.text = "";
            }
            GC.ToggleChat();
        }
    }
    #endregion

    [ClientRpc]
    public void RpcUpdateMatch(MatchData data)
    {
        GC.MatchDataUpdate(data);
    }

}
