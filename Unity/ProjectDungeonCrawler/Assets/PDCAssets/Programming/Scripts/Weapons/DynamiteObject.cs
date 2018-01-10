﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamiteObject : NetworkBehaviour, IHitable
{
    public float damage = 150f;
    public float fuseTime = 5f;
    public float explosionRadius = 25f;
    public float explosionForce = 100000f;
    public LayerMask layermask;
    public GameObject explosionParticle;

    float timer;
    bool exploded = false;
    string hitter;
    string hitterID;

    public string objectID
    {
        get
        {
            return "Dynamite";
        }
    }

    public string objectName
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public NetworkInstanceId networkID
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public void SetOwner(string _hitter, string _hitterID)
    {
        hitter = _hitter;
        hitterID = _hitterID;
    }

    public void LightFuse()
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
        yield return new WaitForSeconds(.5f);
        if(isServer)
            NetworkServer.Destroy(this.gameObject);
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            if (isServer)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, layermask);
                foreach(Collider hit in hits)
                {
                    hit.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    float perc = 1 - distance / explosionRadius;
                    print("My perc is: " + perc);
                    byte newDmg = (byte)(damage * perc);
                    print("My dmg is: " + newDmg);
                    hit.attachedRigidbody.GetComponent<NWPlayerCombat>().CmdGetHit(new NetworkPackages.DamagePackage(newDmg, hitter, hitterID, hit.transform.position));
                }
            }
        }
    }

    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        Explode();
    }
}
