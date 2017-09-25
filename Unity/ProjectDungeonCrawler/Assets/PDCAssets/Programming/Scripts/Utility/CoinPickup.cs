using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.UI;

public class CoinPickup : MonoBehaviour {

    AudioSource audioS;
    public MeshRenderer render;
    public Collider col;

    private void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.isTrigger)
        {
            render.enabled = false;
            col.enabled = false;
            audioS.Play();
            UIManager.instance.AddProgress();
            Destroy(col.gameObject, 5);
        }
    }
}
