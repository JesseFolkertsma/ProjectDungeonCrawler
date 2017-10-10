using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStats : NetworkBehaviour {
    public int id;
    public string playerName = "New player";
    public float maxHP = 100;
    public float hp = 100;
}
