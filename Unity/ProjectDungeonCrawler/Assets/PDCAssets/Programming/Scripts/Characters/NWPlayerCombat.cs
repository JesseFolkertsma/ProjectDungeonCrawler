using System;
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

    [SyncVar] public float testHP = 100;

    //public variables
    public bool isActive = true;
    public int equippedWeapon;
    public Animator weaponHolderAnim;
    public Transform weaponHolder;
    public Transform offSetObject;
    public GameObject canvas;
    public NetworkedUI netUI;
    public WeaponState state = WeaponState.Idle;


    //private serializable
    [SerializeField] GameObject ragdoll;
    [SerializeField] SkinnedMeshRenderer[] visuals;
    [SerializeField] string playerName;
    [SerializeField] bool isDead;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] Collider[] playercolliders;
    [SerializeField] Weapon[] weapons;
    [SerializeField] GameObject blood;
    [SerializeField] GameObject dynamite;

    //Private Variables
    float shootTimer = 0f;
    float wepTimer = 0f;
    float timer;
    bool mouseDown = false;
    bool[] wasEnabled;
    GeneralCanvas hud;
    NetworkedController controller;

    Weapon equipped
    {
        get
        {
            return weapons[equippedWeapon];
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
        SetDefaults();
        SetEquippedWeapon(0, true);

        foreach (Weapon w in weapons)
        {
            w.Setup(this, isLocalPlayer);
        }

        if (!isLocalPlayer)
        {
            return;
        }

        //Setup for local player

        //Setup name and weapon for all instances of the player
        playerName = PlayerInfo.instance.playerName;
        CmdSetName(PlayerInfo.instance.playerName);
        hud = Instantiate(canvas).GetComponentInChildren<GeneralCanvas>();
        netUI = GetComponent<NetworkedUI>();
        netUI.Init(this);
        CmdJoinMatch(gameObject.name, PlayerInfo.instance.playerName);
        CmdEquipWeapon(0, 0);

        foreach (NWPlayerCombat pc in FindObjectsOfType<NWPlayerCombat>())
        {
            if (pc != this)
                pc.EquipWeapon(pc.equippedWeapon);
        }
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
            CmdButtonUp();
            mouseDown = false;
        }

        if (Input.GetButtonDown("Reload"))
        {
            Reload();
        }
        if (Input.GetButtonDown("Throw"))
        {
            CmdThrowDynamite();
            print("a");
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
        if (!equipped.data.canHoldMouseDown && mouseDown)
            return;

        if (state != WeaponState.Idle)
            return;

        if (equipped.data.currentAmmo > 0)
        {
            if (equipped.timer < Time.time)
            {
                equipped.timer = Time.time + 1 / equipped.data.attackRate;
                AttackEffect();
            }
        }
        else
        {
            if (!equipped.canReload)
            {
                CmdButtonUp();
                CmdEquipWeapon(0, 0);
            }
            else
            {
                Reload();
            }
        }
    }

    public void AttackEffect()
    {
        equipped.data.currentAmmo--;
        equipped.PlayVisuals();
        GeneralCanvas.canvas.CHSpread(equipped.data.recoilIntensity);
        GeneralCanvas.canvas.SetAmmoCount(true, true, equipped.data.maxAmmo, equipped.data.currentAmmo);
        float rngHeight = UnityEngine.Random.Range(-.02f, .02f);
        float rngWidth = UnityEngine.Random.Range(-.02f, .02f);
        Transform cam = controller.playerCam.transform;
        Vector3 rngDir = cam.forward + cam.up * rngHeight + cam.right * rngWidth;
        Ray newRay = new Ray(cam.position, rngDir);
          
        WeaponUtility.IHitableHit iHit = WeaponUtility.GetEnemiesInAttack(equipped.data, newRay);
        if (iHit == null)
        {
            //Hit Nothing
            CmdMuzzleFlash();
            return;
        }

        if (iHit.iHit == null)
        {
            //Hit EnviromentObject
            Vector3 hitpos = iHit.rayHit.point + iHit.rayHit.normal * .01f;
            Quaternion hitrot = Quaternion.LookRotation(-iHit.rayHit.normal);
            equipped.WeaponEffects(hitpos, hitrot);
            CmdEnviromentHit(hitpos, hitrot);
            return;
        }

        NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage((byte)equipped.data.damage, objectName, gameObject.name, iHit.rayHit.point);
        if (PlayerManager.PlayerExists(iHit.iHit.objectID))
        {
            hud.HitMark();
            string playerID = iHit.iHit.objectID;
            Debug.Log("I wil damage: " + playerID.ToString());
            //Damage player
            CmdPlayerHit(playerID, dPck);
        }
        else
        {
            CmdMuzzleFlash();
            iHit.rayHit.transform.GetComponent<DestroyableObject>().Replace();
        }
    }

    void Reload()
    {
        if (!equipped.canReload || state == WeaponState.Reloading || equipped.data.currentAmmo == equipped.data.maxAmmo) return;

        if (reloadRoutine == null)
        {
            state = WeaponState.Reloading;
            weaponHolderAnim.SetTrigger("Reload");
            reloadRoutine = StartCoroutine(ReloadRoutine());
        }
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
        reloadRoutine = null;
    }

    Coroutine equipRoutine;
    IEnumerator EquipRoutine(int wepID)
    {
        if(isLocalPlayer)
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
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.SetAmmoCount(false, true, weapons[equippedWeapon].data.maxAmmo, weapons[equippedWeapon].data.currentAmmo);
            GeneralCanvas.canvas.CHChange(wepID);
        }
        state = WeaponState.Idle;
    }

    public void EquipFromPickup(int weapID, int category,int pickupID)
    {
        CmdEquipWeapon((byte)weapID, (byte)pickupID);
    }
    
    public void EquipWeapon(int weapon)
    {
        weaponHolderAnim.SetTrigger("Equip");
        weapons[equippedWeapon].gameObject.SetActive(false);
        equippedWeapon = weapon;
        weapons[equippedWeapon].gameObject.SetActive(true);
        StartCoroutine(EquipRoutine(weapon));
        controller.rightIKPos = weapons[equippedWeapon].rightIK;
        controller.leftIKPos = weapons[equippedWeapon].leftIK;
        equipped.data.currentAmmo = equipped.data.maxAmmo;
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
            CmdEquipWeapon(0, 0);
        }
        foreach (SkinnedMeshRenderer render in visuals)
        {
            render.enabled = true;
        }

        Debug.Log(transform.name + " has respawned!");
    }


    void Die()
    {
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.DeathscreenActivate(true);
        }

        isDead = true;
        foreach(SkinnedMeshRenderer render in visuals)
        {
            render.enabled = false;
        }
        Instantiate(ragdoll, transform.position, transform.rotation);

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        StartCoroutine(Respawn());
    }

    //Server Commands
    #region Commands
    [Command(channel = 4)]
    void CmdThrowDynamite()
    {
        GameObject newDynamite = Instantiate(dynamite, controller.playerCam.transform.position, controller.playerCam.transform.rotation);
        NetworkServer.Spawn(newDynamite);
    }

    [Command(channel =3)]
    void CmdJoinMatch(string id, string name)
    {
        MatchManager.instance.JoinMatch(id, name);
    }

    [Command(channel = 2)]
    void CmdPlayerKilled(string killerID, string victimID)
    {
        MatchManager.instance.PlayerKilled(killerID, victimID);
    }
    #endregion

    //Gameplay Unet functions
    #region Unet

    [Command(channel = 3)]
    void CmdSetName(string name)
    {
        RpcSetName(name);
    }

    [ClientRpc(channel = 3)]
    void RpcSetName(string name)
    {
        playerName = name;
    }

    [Command(channel = 1)]
    void CmdMuzzleFlash()
    {
        RpcMuzzleFlash();
    }

    [ClientRpc(channel = 1)]
    void RpcMuzzleFlash()
    {
        if(!isLocalPlayer)
            equipped.PlayVisuals();
    }

    [Command(channel = 1)]
    void CmdEnviromentHit(Vector3 hitpos, Quaternion hitrot)
    {
        RpcEnviromentHit(hitpos, hitrot);
    }

    [ClientRpc(channel = 1)]
    void RpcEnviromentHit(Vector3 hitpos, Quaternion hitrot)
    {
        if (!isLocalPlayer)
        {
            equipped.PlayVisuals();
            weapons[equippedWeapon].WeaponEffects(hitpos, hitrot);
        }
    }

    [Command(channel = 3)]
    void CmdPlayerHit(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        RpcMuzzleFlash();
        PlayerManager.GetPlayer(playerID).RpcGetHit(dmgPck);
    }

    [ClientRpc(channel = 3)]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        if (isDead)
            return;

        testHP -= dmgPck.damage;
        Instantiate(blood, dmgPck.hitPosition, Quaternion.identity);
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

    [ClientRpc(channel = 3)]
    public void RpcDie()
    {
        Die();
    }

    [Command(channel = 1)]
    void CmdEquipWeapon(byte weapon, byte pickupID)
    {
        RpcEquipWeapon(weapon, pickupID);
    }

    [ClientRpc(channel = 1)]
    void RpcEquipWeapon(byte weapon, byte pickupID)
    {
        EquipWeapon(weapon);
        if (pickupID != 0)
        {
            foreach (PickUp pu in FindObjectsOfType<PickUp>())
            {
                if (pu.id == pickupID)
                {
                    pu.Respawn();
                }
            }
        }
    }

    [Command(channel = 4)]
    void CmdAttack()
    {
        RpcAttack();
    }

    [ClientRpc(channel = 3)]
    void RpcAttack()
    {
        equipped.Attack();
    }

    [Command(channel = 4)]
    void CmdButtonUp()
    {
        RpcButtonUp();
    }

    [ClientRpc(channel = 4)]
    void RpcButtonUp()
    {
        equipped.AttackButtonUp();
    }
    #endregion
}
