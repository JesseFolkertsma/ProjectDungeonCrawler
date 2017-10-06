using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    const string PLAYER_ID_PREFIX = "Player ";

    static Dictionary<string, NWPlayerCombat> players = new Dictionary<string, NWPlayerCombat>();

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;

    public static void RegisterPlayer(string playerID, NWPlayerCombat player)
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

    public static NWPlayerCombat GetPlayer(string playerID)
    {
        NWPlayerCombat pc = players[playerID];
        return pc;
    }
}