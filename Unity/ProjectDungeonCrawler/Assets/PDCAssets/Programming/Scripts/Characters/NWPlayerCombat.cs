using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour, IHitable
{

    //public variables
    [SyncVar] public float testHP = 100;
    public Inventory inv;
    public bool isActive = true;
    public Transform weaponHolder;
    public NetworkedGunFX networkFX;
    public GameObject canvas;

    //private serializable
    [SerializeField] string playerName;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] Collider[] playercolliders;
    [SerializeField] bool isDead;
    [SerializeField] RigidbodyConstraints deadRBC;

    //private variables
    int equippedWeapon;
    WeaponData equippedData;
    List<GunVisuals> weaponVisuals;

    bool[] wasEnabled;
    bool mouseDown = false;
    float timer;

    NetworkedController controller;
    HUDManager hud;

    RigidbodyConstraints originalRBC;

    public int EquippedWeapon
    {
        get
        {
            return equippedWeapon;
        }
        set
        {
            equippedWeapon = value;
            equippedData = WeaponDatabase.instance.GetWeapon(inv.weapons[equippedWeapon]);
        }
    }

    public NetworkInstanceId networkID
    {
        get
        {
            return netId;
        }
    }

    public string objectID
    {
        get
        {
            return gameObject.name;
        }
    }

    public string objectName
    {
        get
        {
            return playerName;
        }
    }

    public void Setup()
    {
        //Setup for every instance of the player on all clients
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
            weaponVisuals = new List<GunVisuals>();
            GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[equippedWeapon]);
            wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            GunVisuals wv = wepGO.GetComponent<GunVisuals>();
            weaponVisuals.Add(wv);
            if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
                equippedWeapon = 0;
            return;
        }

        //Setup for local player
        weaponVisuals = new List<GunVisuals>();
        for (int i = 0; i < inv.weapons.Count; i++)
        {
            GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[i]);
            wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            GunVisuals wv = wepGO.GetComponent<GunVisuals>();
            weaponVisuals.Add(wv);
            weaponVisuals[i].gameObject.SetActive(false);
            wv.Setup(this);
        }

        //Setup name and weapon for all instances of the player
        CmdSetName(GameObject.FindObjectOfType<PlayerInfo>().playerName);
        CmdSpawnClientWeps();

        if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
            equippedWeapon = 0;

        if(equippedData == null)
            equippedData = WeaponDatabase.instance.GetWeapon(inv.weapons[equippedWeapon]);
    
        weaponVisuals[equippedWeapon].gameObject.SetActive(true);
        hud = Instantiate(canvas).GetComponentInChildren<HUDManager>();
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
        if (Input.GetButton("Fire1"))
        {
            Attack();
            mouseDown = true;
        }
        else
        {
            mouseDown = false;
        }
    }

    void Attack()
    {
        if (!isLocalPlayer)
            return;

        if (!equippedData.canHoldMouseDown && mouseDown)
            return;

        GunVisuals wv = weaponVisuals[equippedWeapon];
        if(wv != null)
        {
            wv.Attack();
        }
    }

    public void DoAttackEffect()
    {
        if (!isLocalPlayer)
            return;
        
        CmdSendFX();
        IHitable[] iHits = WeaponUtility.GetEnemiesInAttack(equippedData, controller.playerCam.transform);

        foreach (IHitable iHit in iHits)
        {
            NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(equippedData.damage, objectName);
            if (PlayerManager.PlayerExists(iHit.objectID))
            {
                hud.HitMark();
                string playerID = iHit.objectID;
                Debug.Log("I wil damage: " + playerID.ToString());
                CmdDamageClient(playerID, dPck);
            }
            else
            {
                CmdDamageServer(dPck, iHit.networkID);
            }
        }
    }

    [Command]
    void CmdSetName(string name)
    {
        RpcSetName(name);
    }

    [ClientRpc]
    void RpcSetName(string name)
    {
        playerName = name;
        Debug.Log("ID: " + objectID + ", Name: " + objectName);
    }

    [ClientRpc]
    void RpcGetFX()
    {
        weaponVisuals[equippedWeapon].RemotePlayerEffectVisuals();
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
        if (weaponVisuals != null) return;

        weaponVisuals = new List<GunVisuals>();
        GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[equippedWeapon]);
        wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        GunVisuals wv = wepGO.GetComponent<GunVisuals>();
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
    void CmdSendMessage(string message, string sender, bool dontSendToSender)
    {
        foreach (KeyValuePair<string, NWPlayerCombat> kvp in PlayerManager.PlayerList())
        {
            if (kvp.Key == sender && dontSendToSender) continue;
            kvp.Value.TargetRecieveMessage(kvp.Value.connectionToClient, message);
        }
    }

    [TargetRpc]
    void TargetRecieveMessage(NetworkConnection conn, string message)
    {
        hud.FeedMessage(message);
    }

    [ClientRpc]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        if (isDead)
            return;

        testHP -= dmgPck.damage;
        print("Is me " + playerName + "! And i hit hit with " + dmgPck.damage.ToString() +" damage by " + dmgPck.hitter + "! I now have " + testHP.ToString() + " health.");
        if (isLocalPlayer)
            hud.UpdateHealth(testHP, 100);
        if (testHP <= 0)
        {
            Die();
            if (isLocalPlayer)
                CmdSendMessage(dmgPck.hitter + " has killed " + objectName, objectID, false);
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
        if(isLocalPlayer)
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
