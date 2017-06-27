using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

public class ItemPointer : MonoBehaviour {

    Transform player;
    public float hoverHeight = 1;

    private void Start()
    {
        StartCoroutine(SearchPlayer());
    }

    IEnumerator SearchPlayer()
    {
        while (player == null)
        {
            player = FindObjectOfType<PlayerController>().transform;
            yield return new WaitForSeconds(3);
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 pPos = player.position;
            pPos.y = transform.position.y;
            transform.LookAt(pPos);

            transform.localPosition = Vector3.zero;
            transform.position += Vector3.up * hoverHeight;
        }
    }
}
