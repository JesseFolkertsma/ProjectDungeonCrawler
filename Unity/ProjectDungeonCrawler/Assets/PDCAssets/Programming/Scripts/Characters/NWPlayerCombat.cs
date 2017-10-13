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
    public NetworkedGunFX networkFX;
    public GameObject canvas;

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
    HUDManager hud;

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

        if (!isLocalPlayer)
        {
            weaponVisuals = new List<WeaponVisuals>();
            GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[equippedWeapon]);
            wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            WeaponVisuals wv = wepGO.GetComponent<WeaponVisuals>();
            weaponVisuals.Add(wv);
            if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
                equippedWeapon = 0;
            return;
        }

        weaponVisuals = new List<WeaponVisuals>();
        for (int i = 0; i < inv.weapons.Count; i++)
        {
            GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[i]);
            wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            WeaponVisuals wv = wepGO.GetComponent<WeaponVisuals>();
            weaponVisuals.Add(wv);
            weaponVisuals[i].gameObject.SetActive(false);
        }
        CmdSpawnClientWeps();

        if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
            equippedWeapon = 0;

        weaponVisuals[equippedWeapon].gameObject.SetActive(true);
        hud = Instantiate(canvas).GetComponent<HUDManager>();
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
        }
    }

    void Attack()
    {
        if (!isLocalPlayer) return;

        WeaponVisuals wv = weaponVisuals[equippedWeapon];
        if(wv != null)
        {
            CmdSendFX();
            wv.ShootVisuals();
            WeaponData weaponData = WeaponDatabase.instance.GetWeapon(inv.weapons[equippedWeapon]);
            IHitable[] iHits = WeaponUtility.GetEnemiesInAttack(weaponData, controller.playerCam.transform);
            
            foreach (IHitable iHit in iHits)
            {
                NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(weaponData.damage, objectName);
                if (PlayerManager.PlayerExists(iHit.objectName))
                {
                    hud.HitMark();
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

    [ClientRpc]
    void RpcGetFX()
    {
        weaponVisuals[equippedWeapon].ShootVisuals();
    }

    [Command]
    void CmdSendFX()
    {
        RpcGetFX();
    }

    [Command]
    void CmdSpawnClientWeps()
    {
        RpcSpawnClientWeps();
    }

    [ClientRpc]
    void RpcSpawnClientWeps()
    {
        if (isLocalPlayer) return;
        weaponVisuals = new List<WeaponVisuals>();
        GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[equippedWeapon]);
        wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        WeaponVisuals wv = wepGO.GetComponent<WeaponVisuals>();
        weaponVisuals.Add(wv);
        if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
            equippedWeapon = 0;
    }

    [Command]
    public void CmdSpawnObjectOnServer(int objID, Vector3 position, Quaternion rotation)
    {
        GameManager.instance.SpawnObjectOnServer(objID, position, rotation);
    }

    [Command]
    void CmdDamageServer(NetworkPackages.DamagePackage dmgPck, NetworkInstanceId objectID)
    {
        NetworkServer.FindLocalObject(objectID).GetComponent<IHitable>().RpcGetHit(dmgPck);
    }

    [Command]
    void CmdDamageClient(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        PlayerManager.GetPlayer(playerID).RpcGetHit(dmgPck);
    }

    [Command]
    void CmdSendMessage(string message)
    {

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
            hud.UpdateHealth(0, 100);
            return;
        }
        if(!isLocalPlayer)
            hud.UpdateHealth(testHP, 100);
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
        hud.UpdateHealth(100, 100);

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
