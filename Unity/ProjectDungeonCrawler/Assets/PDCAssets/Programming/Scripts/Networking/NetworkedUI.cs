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
    [Command(channel = 1)]
    public void CmdFeedMessage(string message) {
        RpcFeedMessage(message);
    }
    [ClientRpc(channel = 1)]
    public void RpcFeedMessage(string message) {
        GC.FeedMessage(message);
    }
    #endregion
    #region Chat
    //Variables//
    [Command(channel = 1)]
    public void CmdChatMessage(string message) {
        RpcChatMessage(message);
    }
    [ClientRpc(channel = 1)]
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

    [ClientRpc(channel = 2)]
    public void RpcUpdateMatch(MatchData data)
    {
        GC.MatchDataUpdate(data);
    }

    [ClientRpc]
    public void RpcEndMatch()
    {
        GC.MatchEnd();
    }

    [ClientRpc]
    public void RpcResetMatch()
    {
        GC.ResetMatch();
    }
}
