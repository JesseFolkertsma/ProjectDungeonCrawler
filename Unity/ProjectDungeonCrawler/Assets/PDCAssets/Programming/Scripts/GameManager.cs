using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;

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