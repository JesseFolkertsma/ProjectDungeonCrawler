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

    public void AddEntry(string name, string id)
    {
        CmdAddEntry(name, id);
    }

    [Command]
    public void CmdAddEntry(string name, string id)
    {
        RpcAddEntry(name, id);
    }

    [ClientRpc]
    public void RpcAddEntry(string name, string id)
    {
        GeneralCanvas.canvas.AddScoreBoardEntry(name, id);
    }

    public void UpdateEntry(string id, int kills, int deaths)
    {
        CmdUpdateEntry(id, kills, deaths);
    }

    [Command]
    public void CmdUpdateEntry(string id, int kills, int deaths)
    {
        RpcUpdateEntry(id, kills, deaths);
    }

    [ClientRpc]
    public void RpcUpdateEntry(string id, int kills, int deaths)
    {
        GeneralCanvas.canvas.AddScoreBoardStat(id, kills, deaths);
    }

    [ClientRpc]
    public void RpcUpdateMatch(MatchData data)
    {
        GeneralCanvas.canvas.MatchDataUpdate(data);
    }

}
