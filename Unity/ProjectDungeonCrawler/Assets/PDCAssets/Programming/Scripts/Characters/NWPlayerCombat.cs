using System;
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
    public Transform weaponHolder;

    int equippedWeapon;
    List<WeaponVisuals> weaponVisuals;

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

        //ComponentSetup
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled; 
        }
        originalRBC = controller.rb.constraints;
        SetDefaults();

        weaponVisuals = new List<WeaponVisuals>();

        if (NetworkServer.active)
        {
            Debug.Log("ACTIVE SERVER!");
        }
        else
        {
            Debug.LogError("SERVER NOT ACTIVE KILL YOURSELF!");
        }

        CmdSpawnWeaponsForPlayer(inv.weapons.ToArray(), gameObject.name);

        if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
            equippedWeapon = 0;

        weaponVisuals[equippedWeapon].gameObject.SetActive(true);
    }

    [Command]
    void CmdSpawnWeaponsForPlayer(int[] _inv, string playerID)
    {

        for (int i = 0; i < _inv.Length; i++)
        {
            GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(_inv[i]);
            wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            NWPlayerCombat pc = GameManager.instance.GetPlayer(playerID);
            NetworkServer.SpawnWithClientAuthority(wepGO, pc.gameObject);
            WeaponVisuals wv = wepGO.GetComponent<WeaponVisuals>();
            pc.weaponVisuals.Add(wv);
            pc.weaponVisuals[i].gameObject.SetActive(false);
        }
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
        if(weaponVisuals[equippedWeapon] != null)
        {
            weaponVisuals[equippedWeapon].CmdShootVisuals();
            WeaponData weaponData = WeaponDatabase.instance.GetWeapon(inv.weapons[equippedWeapon]);
            IHitable[] iHits = WeaponUtility.GetEnemiesInAttack(weaponData, controller.playerCam.transform);

            Debug.Log("haswep");
            foreach (IHitable iHit in iHits)
            {
                Debug.Log("hit");
                NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(weaponData.damage, objectName);
                if (GameManager.instance.PlayerExists(iHit.objectName))
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
        GameManager.instance.GetPlayer(playerID).RpcGetHit(dmgPck);
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

    [Command]
    void CmdSetAuthority(NetworkIdentity id, NetworkConnection conn)
    {
        id.AssignClientAuthority(conn);
    }

    [Command]
    void SpawnWeapons(Inventory _inv)
    {
    }
}
