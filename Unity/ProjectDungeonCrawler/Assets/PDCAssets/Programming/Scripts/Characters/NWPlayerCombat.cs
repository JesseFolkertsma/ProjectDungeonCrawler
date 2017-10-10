﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour, IHitable
{
    [SyncVar]
    public float testHP = 100;
    public Inventory inv;
    public bool isActive = true;

    [SerializeField]
    Behaviour[] disableOnDeath;
    [SerializeField]
    Collider[] playercolliders;
    [SerializeField]
    bool isDead;
    bool[] wasEnabled;
    RigidbodyConstraints originalRBC;
    [SerializeField]
    RigidbodyConstraints deadRBC;

    NetworkedController controller;

    public NetworkInstanceId networkID
    {
        get
        {
            return netId;
        }
    }

    public string objectName
    {
        get
        {
            return gameObject.name;
        }
    }

    public void Setup()
    {
        controller = GetComponent<NetworkedController>();
        inv = GetComponent<Inventory>();

        //ComponentSetup
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled; 
        }
        originalRBC = controller.rb.constraints;
        SetDefaults();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!isActive)
            return;

        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
            Debug.Log("Click");
        }
    }

    void Attack()
    {
        if(inv.equippedWeapon != null)
        {
            IHitable[] iHits = WeaponUtility.GetEnemiesInAttack(inv.equippedWeapon, controller.playerCam.transform);

            Debug.Log("haswep");
            foreach (IHitable iHit in iHits)
            {
                Debug.Log("hit");
                NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(inv.equippedWeapon.instance.stats.damage, objectName);
                if (GameManager.PlayerExists(iHit.objectName))
                {
                    string playerID = iHit.objectName;
                    Debug.Log("I wil damage: " + playerID.ToString());
                    CmdDamageClient(playerID, dPck);
                }
                else
                {
                    CmdDamageServer(dPck, iHit.networkID);
                }
            }
        }
    }

    [Command]
    void CmdDamageServer(NetworkPackages.DamagePackage dmgPck, NetworkInstanceId objectID)
    {
        Debug.Log("cmdserver");
        NetworkServer.FindLocalObject(objectID).GetComponent<IHitable>().RpcGetHit(dmgPck);
    }

    [Command]
    void CmdDamageClient(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        Debug.Log("cmdclient");
        GameManager.GetPlayer(playerID).RpcGetHit(dmgPck);
    }

    [ClientRpc]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        if (isDead)
            return;

        testHP -= dmgPck.damage;
        print("Is me " + objectName + "! And i hit hit with " + dmgPck.damage.ToString() +" damage by " + dmgPck.hitter + "! I now have " + testHP.ToString() + " health.");
        if(testHP <= 0)
        {
            Die();
        }
    }

    void SetDefaults()
    {
        isDead = false;
        testHP = 100;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        foreach(Collider col in playercolliders)
        {
            col.enabled = true;
        }

        controller.rb.constraints = originalRBC;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        SetDefaults();
        Transform newSpawnLocation = NetworkManager.singleton.GetStartPosition();
        transform.position = newSpawnLocation.position;
        transform.rotation = newSpawnLocation.rotation;

        Debug.Log(transform.name + "! I has respawned!");
    }

    void Die()
    {
        isDead = true;
        
        controller.rb.constraints = deadRBC;
        controller.rb.AddExplosionForce(10000, transform.position - transform.up + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)), 10);

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        StartCoroutine(Respawn());
    }
}
