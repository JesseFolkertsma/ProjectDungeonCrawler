using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {

    AudioSource audioS;

    private void Start()
    {
        audioS = transform.parent.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        transform.parent.GetComponent<MeshRenderer>().enabled = false;
        transform.parent.GetComponent<MeshCollider>().enabled = false;
        audioS.Play();
        Destroy(gameObject, 5);
    }
}
