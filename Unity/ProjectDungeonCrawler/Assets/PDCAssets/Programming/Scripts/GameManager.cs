using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    const string PLAYER_ID_PREFIX = "Player ";

    static Dictionary<string, NetworkedController> players = new Dictionary<string, NetworkedController>();

    public static void RegisterPlayer(string playerID, NetworkedController player)
    {
        string id = PLAYER_ID_PREFIX + playerID;
        players.Add(id, player);
        player.gameObject.name = id;
        Debug.Log(id + " joined the game!");
    }

    public static void RemovePlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static NetworkedController GetPlayer(string playerID)
    {
        NetworkedController pc = players[playerID];
        return pc;
    }
}
