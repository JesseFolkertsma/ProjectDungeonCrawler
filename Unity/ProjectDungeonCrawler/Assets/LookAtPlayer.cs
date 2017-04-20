using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {
    Transform player;

    void Start()
    {
        player = FindObjectOfType<PlayerCharacter>().transform;
    }

    void Update()
    {
        transform.LookAt(player);
    }
}
