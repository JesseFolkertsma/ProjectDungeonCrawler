using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkedUI : NetworkBehaviour {
    public void Init()
    {
        CmdFeedMessage(gameObject.name + " has joined the lobby!");
    }
    #region Feed/Notifications
    [Command]
    public void CmdFeedMessage(string message) {
        RpcFeedMessage(message);
    }
    [ClientRpc]
    public void RpcFeedMessage(string message) {
        GeneralCanvas.canvas.FeedMessage(message);
    }
#endregion
}
