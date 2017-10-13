using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    const string PLAYER_ID_PREFIX = "Player ";

    public static GameManager instance;
    static Dictionary<string, NWPlayerCombat> players = new Dictionary<string, NWPlayerCombat>();

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;

    [Header("Prefabs")]
    public GameObject audioObject;
    public GameObject[] spawns;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

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

    public bool PlayerExists(string playerID)
    {
        if (players.ContainsKey(playerID)) return true;
        return false;
    }

    public void SpawnObjectOnServer(int id, Vector3 position, Quaternion rotation)
    {
        GameObject _obj = Instantiate(spawns[id], position, rotation);
        NetworkServer.Spawn(_obj);
    }

    public void SpawnAudioObject(AudioClip audio, float volume, Transform position)
    {
        AudioObject ao = Instantiate(audioObject, position.position, position.rotation).GetComponent<AudioObject>();
        ao.Play(audio, volume);
        NetworkServer.Spawn(ao.gameObject);
    }
}