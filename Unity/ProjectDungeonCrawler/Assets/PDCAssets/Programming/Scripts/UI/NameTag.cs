using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour {
    private Transform player;

    private void Start() {
        Setup();
    }
    public void Setup() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players) {
            if (p.layer == LayerMask.NameToLayer("LocalPlayer")){
                player = p.transform;
            }
        }
    }

    void Update () {
        if (player != null) {
            transform.LookAt(player);
        }
	}
}
