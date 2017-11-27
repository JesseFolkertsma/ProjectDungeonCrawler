﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour, IHitable
{
    public enum WeaponState
    {
        Idle,
        Attacking,
        Reloading,
        Equipping,
    }

    //public variables
    [SyncVar] public float testHP = 100;
    public bool isActive = true;
    public Animator weaponHolderAnim;
    public Transform weaponHolder;
    public Transform offSetObject;
    public GameObject canvas;
    public int equippedWeapon;
    public WeaponState state = WeaponState.Idle;


    //private serializable
    [SerializeField] string playerName;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] Collider[] playercolliders;
    [SerializeField] bool isDead;
    [SerializeField] RigidbodyConstraints deadRBC;
    [SerializeField] Weapon[] weapons;

    float shootTimer = 0f;
    bool[] wasEnabled;
    bool mouseDown = false;
    float timer;
    GeneralCanvas hud;
    public NetworkedUI netUI;

    NetworkedController controller;

    RigidbodyConstraints originalRBC;

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

    public void Init()
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
        SetEquippedWeapon(0, true);

        if (!isLocalPlayer)
        {
            return;
        }

        //Setup for local player
        foreach(Weapon w in weapons)
        {
            w.Setup(this);
        }

        //Setup name and weapon for all instances of the player
        playerName = PlayerInfo.instance.playerName;
        CmdSetName(PlayerInfo.instance.playerName);
        hud = Instantiate(canvas).GetComponentInChildren<GeneralCanvas>();
        netUI = GetComponent<NetworkedUI>();
        netUI.Init(this);
        CmdJoinMatch(gameObject.name, PlayerInfo.instance.playerName);
    }

    [Command]
    void CmdJoinMatch(string id, string name)
    {
        MatchManager.instance.JoinMatch(id, name);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!isActive)
            return;

        CheckInput();
        WeaponEffects();
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
            weapons[equippedWeapon].AttackButtonUp();
            mouseDown = false;
        }

        if (Input.GetButtonDown("Reload"))
        {
            Reload();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            int wepToEquip = equippedWeapon + 1;
            if(wepToEquip > weapons.Length -1)
            {
                wepToEquip = 0;
            }
            EquipWeapon(wepToEquip);
        }
    }

    void WeaponEffects()
    {
        WeaponSway();
        WeaponAnimation();
    }

    void WeaponAnimation()
    {
        float walkF = controller.acc / controller.acceleration;
        if (!controller.grounded)
            walkF = Mathf.Lerp(walkF, 0, Time.deltaTime * 6);
        weaponHolderAnim.SetFloat("Walk", walkF);
    }

    void WeaponSway()
    {
        if (weaponHolder)
        {
            Vector3 sway = new Vector3(-(Mathf.Clamp(Input.GetAxisRaw("Mouse X"), -1, 1) + controller.xInput) / 14, -controller.rb.velocity.y / 20, 0);
            Vector3 newPos = weapons[equippedWeapon].transform.localPosition + sway;
            if (newPos.y > .1f)
                newPos.y = .1f;
            else if (newPos.y < -.1f)
                newPos.y = -.1f;
            offSetObject.localPosition = Vector3.Lerp(offSetObject.localPosition, newPos, Time.deltaTime * 2);
        }
    }

    void Attack()
    {
        if (!isLocalPlayer)
            return;

        if (!weapons[equippedWeapon].data.canHoldMouseDown && mouseDown)
            return;

        if (state != WeaponState.Idle)
            return;

        //if(weapons[equippedWeapon].data.canHoldMouseDown)
        weapons[equippedWeapon].Attack();
    }

    void Reload()
    {
        weaponHolderAnim.SetTrigger("Reload");
        reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    Coroutine reloadRoutine;
    IEnumerator ReloadRoutine()
    {
        GeneralCanvas.canvas.Reloading(true);
        state = WeaponState.Reloading;
        while (weaponHolderAnim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            yield return null;
        }
        while (weaponHolderAnim.GetCurrentAnimatorStateInfo(0).IsTag("Reloading"))
        {
            yield return null;
        }
        weapons[equippedWeapon].data.currentAmmo = weapons[equippedWeapon].data.maxAmmo;
        GeneralCanvas.canvas.SetAmmoCount(false, true, weapons[equippedWeapon].data.maxAmmo, weapons[equippedWeapon].data.currentAmmo);
        state = WeaponState.Idle;

        GeneralCanvas.canvas.Reloading(false);
    }

    Coroutine equipRoutine;
    IEnumerator EquipRoutine()
    {
        GeneralCanvas.canvas.Reloading(false);
        state = WeaponState.Equipping;
        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
        }
        while (weaponHolderAnim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
        {
            yield return null;
        }
        while (weaponHolderAnim.GetCurrentAnimatorStateInfo(0).IsTag("Equipping"))
        {
            yield return null;
        }
        GeneralCanvas.canvas.SetAmmoCount(false, true, weapons[equippedWeapon].data.maxAmmo, weapons[equippedWeapon].data.currentAmmo);
        state = WeaponState.Idle;
    }

    void EquipWeapon(int weapon)
    {
        weaponHolderAnim.SetTrigger("Equip");
        weapons[equippedWeapon].gameObject.SetActive(false);
        equippedWeapon = weapon;
        weapons[equippedWeapon].gameObject.SetActive(true);
        StartCoroutine(EquipRoutine());
    }

    public void DoAttackEffect()
    {
        if (!isLocalPlayer)
            return;
        // Effects

        WeaponUtility.IHitableHit iHit = WeaponUtility.GetEnemiesInAttack(weapons[equippedWeapon].data, controller.playerCam.transform);
        if (iHit.iHit == null)
            CmdWeaponEffects(iHit.rayHit.point, Quaternion.LookRotation(-iHit.rayHit.normal));
        NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(weapons[equippedWeapon].data.damage, objectName, gameObject.name);
        if (iHit.iHit != null)
        {
            if (PlayerManager.PlayerExists(iHit.iHit.objectID))
            {
                hud.HitMark();
                string playerID = iHit.iHit.objectID;
                Debug.Log("I wil damage: " + playerID.ToString());
                //Damage player
                CmdDamageClient(playerID, dPck);
            }
            else
            {
                CmdDamageServer(dPck, iHit.iHit.networkID);
            }
        }
    }

    public bool SetEquippedWeapon(int _weaponInt, bool _override)
    {
        if(!_override && equippedWeapon == 0)
        {
            equippedWeapon = _weaponInt;
            for (int i = 0; i < weapons.Length; i++)
            {
                if(i == _weaponInt)
                {
                    weapons[i].gameObject.SetActive(true);
                }
                else
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }
            return true;
        }
        else if (_override)
        {
            equippedWeapon = _weaponInt;
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i == _weaponInt)
                {
                    weapons[i].gameObject.SetActive(true);
                }
                else
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }
            return true;
        }
        return false;
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
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.DeathscreenActivate(false);
            hud.UpdateHealth(100, 100);
        }

        Debug.Log(transform.name + "! I has respawned!");
    }

    [ClientRpc]
    public void RpcDie()
    {
        Die();
    }

    void Die()
    {
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.DeathscreenActivate(true);
        }

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
    void CmdWeaponEffects(Vector3 hitpos, Quaternion hitrot)
    {
        RpcWeaponEffects(hitpos, hitrot);
    }

    [ClientRpc]
    void RpcWeaponEffects(Vector3 hitpos, Quaternion hitrot)
    {
        weapons[equippedWeapon].WeaponEffects(hitpos, hitrot);
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
    }

    [ClientRpc]
    void RpcGetFX()
    {
        weapons[equippedWeapon].PlayVisuals();
    }

    [Command]
    void CmdSendFX()
    {
        RpcGetFX();
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

    [ClientRpc]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        if (isDead)
            return;

        testHP -= dmgPck.damage;
        print("Is me " + playerName + "! And i hit hit with " + dmgPck.damage.ToString() + " damage by " + dmgPck.hitter + "! I now have " + testHP.ToString() + " health.");
        if (isLocalPlayer)
            hud.UpdateHealth(testHP, 100);
        if (testHP <= 0)
        {
            Die();
            if (isLocalPlayer)
            {
                CmdPlayerKilled(dmgPck.hitterID, gameObject.name);
                netUI.CmdFeedMessage(dmgPck.hitter + " has killed " + objectName + "!");
            }
        }
    }

    [Command]
    void CmdPlayerKilled(string killerID, string victimID)
    {
        MatchManager.instance.PlayerKilled(killerID, victimID);
    }
}
