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
    public int usable;
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
    bool isZoomed = false;
    GeneralCanvas hud;
    NetworkedController controller;
    ClampedCamera camClass;

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
        camClass = GetComponentInChildren<ClampedCamera>();

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
        CmdEquipWeapon(0);

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
            mouseDown = false;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Zoom(!isZoomed);
        }

        if (Input.GetButtonDown("Reload"))
        {
            Reload();
        }
        if (Input.GetButtonDown("Throw"))
        {
            UseUsable();
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

    void Zoom(bool state)
    {
        if (!equipped.IsInBaseState() || !equipped.data.canZoom || isDead)
        {
            state = false;
        }
        isZoomed = state;
        GeneralCanvas.canvas.Zoom = state;
        if(state == true)
        {
            controller.playerCam.fov = 10;
            camClass.sensitivity.x = .2f;
            camClass.sensitivity.y = .2f;
            GeneralCanvas.canvas.CHChange(0);
        }
        else
        {
            controller.playerCam.fov = 70;
            camClass.sensitivity.x = 2;
            camClass.sensitivity.y = 2;
            GeneralCanvas.canvas.CHChange(equippedWeapon + 1);
        }
        equipped.mesh.gameObject.SetActive(!state);
    }

    void Attack()
    {
        if (isDead)
            return;

        if (!equipped.data.canHoldMouseDown && mouseDown)
            return;

        if (state != WeaponState.Idle)
            return;

        if (equipped.data.currentAmmo > 0)
        {
            if (equipped.timer < Time.time)
            {
                equipped.timer = Time.time + 1 / equipped.data.attackRate;
                AttackEffect(1 , equipped.data.spread);
            }
        }
        else
        {
            if (!equipped.canReload)
            {
                CmdEquipWeapon(0);
            }
            else
            {
                Reload();
            }
        }
    }

    public void AttackEffect(int rays, float spread)
    {
        equipped.data.currentAmmo--;
        equipped.PlayVisuals();
        if (equipped.data.canZoom && !isZoomed)
            spread = .15f;
        Zoom(false);
        GeneralCanvas.canvas.CHSpread(equipped.data.spread * 50);
        GeneralCanvas.canvas.SetAmmoCount(true, true, equipped.data.maxAmmo, equipped.data.currentAmmo);
        float rngHeight = UnityEngine.Random.Range(-spread, spread);
        float rngWidth = UnityEngine.Random.Range(-spread, spread);
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
            Debug.Log("SPAWN DECAL");
            Vector3 hitpos = iHit.rayHit.point + iHit.rayHit.normal * .01f;
            Quaternion hitrot = Quaternion.LookRotation(iHit.rayHit.normal);
            string surface = iHit.rayHit.transform.tag.ToString();
            equipped.WeaponEffects(hitpos, hitrot, surface);
            CmdEnviromentHit(hitpos, hitrot, surface);
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
            switch (iHit.iHit.objectID)
            {
                case "Dynamite":
                    //CmdDestroyDynamite(iHit.rayHit.transform.gameObject);
                    break;
                case "Breakable":
                    iHit.rayHit.transform.GetComponent<DestroyableObject>().Replace();
                    break;
            }
        }
    }

    void Reload()
    {
        if (!equipped.canReload || state == WeaponState.Reloading || equipped.data.currentAmmo == equipped.data.maxAmmo) return;

        if (reloadRoutine == null)
        {
            Zoom(false);
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

    public void EquipFromPickup(int weapID, PickUp.Category catagory, int pickupID)
    {
        switch (catagory)
        {
            case PickUp.Category.Weapon:
                CmdEquipWeapon((byte)weapID);
                Zoom(false);
                break;
            case PickUp.Category.Usable:
                usable = weapID;
                GeneralCanvas.canvas.NewUsable(usable);
                break;
        }
        CmdDisablePickup((byte)pickupID);
    }
    
    public void EquipWeapon(int weapon)
    {
        Zoom(false);
        weaponHolderAnim.SetTrigger("Equip");
        weapons[equippedWeapon].gameObject.SetActive(false);
        equippedWeapon = weapon;
        reloadRoutine = null;
        weapons[equippedWeapon].gameObject.SetActive(true);
        StartCoroutine(EquipRoutine(weapon));
        controller.rightIKPos = weapons[equippedWeapon].rightIK;
        controller.leftIKPos = weapons[equippedWeapon].leftIK;
        equipped.data.currentAmmo = equipped.data.maxAmmo;
    }

    Coroutine equipRoutine;
    IEnumerator EquipRoutine(int wepID)
    {
        if (isLocalPlayer)
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
            GeneralCanvas.canvas.CHChange(wepID + 1);
        }
        state = WeaponState.Idle;
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

    public void UseUsable()
    {
        switch (usable)
        {
            case 0:
                GeneralCanvas.canvas.FeedMessage("You have no useable!");
                break;
            case 1:
                CmdThrowDynamite(objectName, gameObject.name);
                break;
            case 2:
                Heal(50);
                break;
        }
        GeneralCanvas.canvas.UseUsable();
        usable = 0;
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
        yield return new WaitForSeconds(2);

        SetDefaults();
        Transform newSpawnLocation = NetworkManager.singleton.GetStartPosition();
        transform.position = newSpawnLocation.position;
        transform.rotation = newSpawnLocation.rotation;
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.DeathscreenActivate(false);
            hud.ResetHealth();
            CmdEquipWeapon(0);
        }
        foreach (SkinnedMeshRenderer render in visuals)
        {
            render.enabled = true;
        }

        Debug.Log(transform.name + " has respawned!");
    }

    public void Heal(int amount)
    {
        StartCoroutine(HealRoutine(amount));
    }

    IEnumerator HealRoutine(int amount)
    {
        int newAmount = amount;
        while (newAmount > 0)
        {
            int amountToHeal = 5;
            if(newAmount < 5)
            {
                amountToHeal = newAmount;
            }
            newAmount -= amountToHeal;
            byte newHP = (byte)(testHP += amountToHeal);
            if (newHP > 100) newHP = 100;
            CmdSetHP(newHP);
            GeneralCanvas.canvas.UpdateHealth(newHP, 100);
            yield return new WaitForSeconds(.5f);
        }
    }

    public void GetHit(NetworkPackages.DamagePackage dmgPck)
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

    void Die()
    {
        if (isLocalPlayer)
        {
            GeneralCanvas.canvas.DeathscreenActivate(true);
            GeneralCanvas.canvas.CHChange(0);
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

    public void EnviromentDeath(DeathTrigger.DeathType type)
    {
        if (isLocalPlayer && !isDead)
        {
            hud.UpdateHealth(testHP, 100);
            CmdPlayerKilled(gameObject.name, gameObject.name);
            Die();
            switch (type)
            {
                case DeathTrigger.DeathType.Water:
                    netUI.CmdFeedMessage(objectName + " has drowned!");
                    break;
            }
        }
    }

    //Server Commands
    #region Commands
    [Command(channel = 1)]
    void CmdSetHP(byte amount)
    {
        testHP = amount;
    }

    [Command(channel = 3)]
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
    [Command(channel = 4)]
    void CmdThrowDynamite(string _owner, string _ownerID)
    {
        GameObject newDynamite = Instantiate(dynamite, controller.playerCam.transform.position + controller.playerCam.transform.forward, controller.playerCam.transform.rotation);
        DynamiteObject dynamiteObject = newDynamite.GetComponent<DynamiteObject>();
        dynamiteObject.GetComponent<Rigidbody>().AddForce(controller.playerCam.transform.forward * 750 + Vector3.up * 100);
        NetworkServer.Spawn(newDynamite);
        RpcThrowDynamite(newDynamite, _owner, _ownerID);
    }

    [ClientRpc(channel = 4)]
    void RpcThrowDynamite(GameObject go, string _owner, string _ownerID)
    {
        go.GetComponent<DynamiteObject>().SetOwner(_owner, _ownerID);
        go.GetComponent<DynamiteObject>().LightFuse();
    }

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
    void CmdEnviromentHit(Vector3 hitpos, Quaternion hitrot, string surface)
    {
        RpcEnviromentHit(hitpos, hitrot, surface);
    }

    [ClientRpc(channel = 1)]
    void RpcEnviromentHit(Vector3 hitpos, Quaternion hitrot, string surface)
    {
        if (!isLocalPlayer)
        {
            equipped.PlayVisuals();
            weapons[equippedWeapon].WeaponEffects(hitpos, hitrot, surface);
        }
    }

    [Command(channel = 3)]
    void CmdPlayerHit(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        RpcMuzzleFlash();
        PlayerManager.GetPlayer(playerID).RpcGetHit(dmgPck);
    }

    [Command(channel = 3)]
    public void CmdGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        RpcGetHit(dmgPck);
    }

    [ClientRpc(channel = 3)]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        GetHit(dmgPck);
    }

    [ClientRpc(channel = 3)]
    public void RpcDie()
    {
        Die();
    }

    [Command(channel = 1)]
    void CmdEquipWeapon(byte weapon)
    {
        RpcEquipWeapon(weapon);
    }

    [ClientRpc(channel = 1)]
    void RpcEquipWeapon(byte weapon)
    {
        EquipWeapon(weapon);
    }

    [Command(channel = 1)]
    void CmdDisablePickup(byte pickupID)
    {
        RpcDisablePickup(pickupID);
    }

    [ClientRpc(channel = 1)]
    void RpcDisablePickup(byte pickupID)
    {
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
    #endregion
}
