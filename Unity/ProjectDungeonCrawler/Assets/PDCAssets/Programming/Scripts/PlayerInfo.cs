using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    string playerName;


	void Start () {
        DontDestroyOnLoad(transform);
	}
    public void UpdateName(string _name) {
        playerName = _name;
    }
}
