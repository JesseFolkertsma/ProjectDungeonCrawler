using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamiteObject : NetworkManager
{
    public float fuseTime = 5f;
    public float explosionRadius = 10;
    public float explosionForce = 500;

    float timer;

    private void Start()
    {
        StartCoroutine(FuseRoutine());
    }

    IEnumerator FuseRoutine()
    {
        timer = Time.time + fuseTime;
        while (Time.time < timer)
        {
            yield return null;
        }

    }

    public void Explode()
    {
        
    }
}
