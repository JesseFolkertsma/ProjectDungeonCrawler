using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {

    public static MyNetworkManager mySingleton;

    private void Awake()
    {
        if (mySingleton == null)
            mySingleton = this;
        else
            Destroy(gameObject);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //PlayerManager.RemovePlayer(conn.playerControllers[0].gameObject.name);
        //MatchManager.instance.LeaveMatch(conn.playerControllers[0].gameObject.name);
        print("A player pressed alt + f4!");
        NetworkServer.Destroy(conn.playerControllers[0].gameObject);
        //base.OnServerDisconnect(conn);
    }
}
