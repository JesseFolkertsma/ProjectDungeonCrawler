﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    const string PLAYER_ID_PREFIX = "Player ";

    static Dictionary<string, NWPlayerCombat> players = new Dictionary<string, NWPlayerCombat>();

    static public void RegisterPlayer(string playerID, NWPlayerCombat player)
    {
        string id = PLAYER_ID_PREFIX + playerID;
        players.Add(id, player);
        player.gameObject.name = id;
        Debug.Log(id + " joined the game!");
    }

    static public void RemovePlayer(string playerID)
    {
        players.Remove(playerID);
    }

    static public NWPlayerCombat GetPlayer(string playerID)
    {
        NWPlayerCombat pc = players[playerID];
        return pc;
    }

    static public bool PlayerExists(string playerID)
    {
        if (players.ContainsKey(playerID)) return true;
        return false;
    }

    static public Dictionary<string, NWPlayerCombat> PlayerList()
    {
        return players;
    }
}
