using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    public string playerName;
    public static PlayerInfo instance;

	void Start () {
        DontDestroyOnLoad(transform);
        instance = this;
	}
}
