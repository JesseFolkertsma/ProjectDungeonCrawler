using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour, IHitable
{
    public float testHP = 100;
    public Inventory inv;
    public bool isActive = true;

    NetworkedController controller;

    public NetworkConnection networkConn
    {
        get
        {
            return connectionToClient;
        }
    }

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

    private void Start()
    {
        controller = GetComponent<NetworkedController>();
        inv = GetComponent<Inventory>();
    }

    private void Update()
    {
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
                NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(inv.equippedWeapon.instance.stats.damage);
                if (iHit.networkConn != null)
                {
                    string playerID = iHit.objectName;
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
        NetworkServer.FindLocalObject(objectID).GetComponent<IHitable>().GetHit(dmgPck);
    }

    [Command]
    void CmdDamageClient(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        Debug.Log("cmdclient");
        TargetGiveDamageToPlayer(GameManager.GetPlayer(playerID).connectionToClient, dmgPck);
    }

    [TargetRpc]
    void TargetGiveDamageToPlayer(NetworkConnection conn, NetworkPackages.DamagePackage dmgPck)
    {
        Debug.Log("target");
        GetHit(dmgPck);
    }

    public void GetHit(NetworkPackages.DamagePackage dmgPck)
    {
        testHP -= dmgPck.damage;
        print("Mah god I got hit!");
    }
}
