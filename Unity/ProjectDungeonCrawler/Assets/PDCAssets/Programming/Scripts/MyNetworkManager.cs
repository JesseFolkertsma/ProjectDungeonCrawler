﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //PlayerManager.RemovePlayer(conn.playerControllers[0].gameObject.name);
        //MatchManager.instance.LeaveMatch(conn.playerControllers[0].gameObject.name);
        print("A player pressed alt + f4 my dude!");
        NetworkServer.Destroy(conn.playerControllers[0].gameObject);
        //base.OnServerDisconnect(conn);
    }
}
