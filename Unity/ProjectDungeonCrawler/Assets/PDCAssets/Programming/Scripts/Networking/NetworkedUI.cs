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
        CmdFeedMessage(_pc.objectName + " has joined the lobby!", _pc.objectName,"");
        pc = _pc;
    }
#endregion
    #region Feed/Notifications
    [Command(channel = 1)]
    public void CmdFeedMessage(string message, string killer, string killed) {
        RpcFeedMessage(message, killer, killed);
    }
    [ClientRpc(channel = 1)]
    public void RpcFeedMessage(string message, string killer, string killed) {
        bool isme = false;
        foreach(NWPlayerCombat pc in FindObjectsOfType<NWPlayerCombat>())
        {
            if (pc.isLocalPlayer)
            {
                if (pc.objectName == killer || pc.objectName == killed)
                {
                    isme = true;
                }
                break;
            }
        }
        GC.FeedMessage(message, isme);
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
    public void RpcUpdateMatch(byte[] data)
    {
        GC.MatchDataUpdate((MatchData)StaticFunctions.ByteArrayToObject(data));
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
