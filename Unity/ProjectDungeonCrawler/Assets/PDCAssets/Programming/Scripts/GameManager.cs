using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    const string PLAYER_ID_PREFIX = "Player ";

    public static GameManager instance;
    Dictionary<string, NWPlayerCombat> players = new Dictionary<string, NWPlayerCombat>();

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;

    [Header("Prefabs")]
    public GameObject audioObject;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPlayer(string playerID, NWPlayerCombat player)
    {
        string id = PLAYER_ID_PREFIX + playerID;
        players.Add(id, player);
        player.gameObject.name = id;
        Debug.Log(id + " joined the game!");
    }

    public void RemovePlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public NWPlayerCombat GetPlayer(string playerID)
    {
        NWPlayerCombat pc = players[playerID];
        return pc;
    }

    public bool PlayerExists(string playerID)
    {
        if (players.ContainsKey(playerID)) return true;
        return false;
    }

    public void SpawnObjectOnServer(GameObject obj, Transform position)
    {
        GameObject _obj = Instantiate(obj, position.position, position.rotation);
        NetworkServer.Spawn(_obj);
    }

    public void SpawnObjectOnServer(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject _obj = Instantiate(obj, position, rotation);
        NetworkServer.Spawn(_obj);
    }

    public void SpawnAudioObject(AudioClip audio, float volume, Transform position)
    {
        AudioObject ao = Instantiate(audioObject, position.position, position.rotation).GetComponent<AudioObject>();
        ao.Play(audio, volume);
        NetworkServer.Spawn(ao.gameObject);
    }
}