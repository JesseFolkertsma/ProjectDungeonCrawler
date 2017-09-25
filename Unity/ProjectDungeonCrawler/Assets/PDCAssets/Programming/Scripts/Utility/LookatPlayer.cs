using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

public class LookatPlayer : MonoBehaviour {

    Transform player;

    private void Start()
    {
        StartCoroutine(SearchPlayer());
    }

    IEnumerator SearchPlayer()
    {
        while(player == null)
        {
            if(FindObjectOfType<PlayerController>() != null)
                player = FindObjectOfType<PlayerController>().transform;
            yield return new WaitForSeconds(3);
        }
    }

    private void Update()
    {
        if(player != null)
        {
            Vector3 pPos = player.position;
            pPos.y = transform.position.y;
            transform.LookAt(pPos);
        }
    }
}
