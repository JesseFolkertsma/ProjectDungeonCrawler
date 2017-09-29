using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    const string PLAYER_ID_PREFIX = "Player ";

    static Dictionary<string, PlayerController> players = new Dictionary<string, PlayerController>();

    public static void RegisterPlayer(string playerID, PlayerController player)
    {
        string id = PLAYER_ID_PREFIX + playerID;
        players.Add(id  , player);
        player.gameObject.name = id;
        Debug.Log(id + " joined the game!");
    }

    public static void RemovePlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static PlayerController GetPlayer(string playerID)
    {
        PlayerController pc = players[playerID];
        return pc;
    }
}
