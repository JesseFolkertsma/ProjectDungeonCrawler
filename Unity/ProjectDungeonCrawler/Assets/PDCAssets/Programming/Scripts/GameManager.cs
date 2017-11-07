using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}