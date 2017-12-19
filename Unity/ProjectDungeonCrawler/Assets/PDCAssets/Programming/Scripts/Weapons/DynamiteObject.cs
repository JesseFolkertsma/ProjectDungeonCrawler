using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamiteObject : NetworkBehaviour
{
    public float fuseTime = 5f;
    public float explosionRadius = 10;
    public float explosionForce = 500;
    public LayerMask layermask;
    public GameObject explosionParticle;

    float timer;
    bool exploded = false;

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
        Explode();
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, layermask);
            foreach(Collider hit in hits)
            {
                hit.attachedRigidbody.AddExplosionForce(explosionForce * 50, transform.position, explosionRadius);
            }
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            if (isServer)
            {
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }
}
