using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour, IHitable
{
    //public variables
    [SyncVar] public float testHP = 100;
    public bool isActive = true;
    public Animator weaponHolderAnim;
    public Transform weaponHolder;
    public Transform offSetObject;
    public GameObject canvas;
    public int equippedWeapon;


    //private serializable
    [SerializeField] string playerName;
    [SerializeField] Behaviour[] disableOnDeath;
    [SerializeField] Collider[] playercolliders;
    [SerializeField] bool isDead;
    [SerializeField] RigidbodyConstraints deadRBC;
    [SerializeField] Weapon[] weapons;

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
        netUI.AddEntry(playerName, gameObject.name);
        foreach(KeyValuePair<string,NWPlayerCombat> kvp in PlayerManager.PlayerList())
        {
            if (kvp.Value.Equals(this)) return;

            GeneralCanvas.canvas.AddScoreBoardEntry(kvp.Value.objectName, kvp.Key);
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

        weapons[equippedWeapon].Attack();
    }

    public void DoAttackEffect()
    {
        if (!isLocalPlayer)
            return;
        // Effects

        WeaponUtility.IHitableHit iHit = WeaponUtility.GetEnemiesInAttack(weapons[equippedWeapon].data, controller.playerCam.transform);
        CmdWeaponEffects(iHit.rayHit.point, Quaternion.LookRotation(iHit.rayHit.normal));
        NetworkPackages.DamagePackage dPck = new NetworkPackages.DamagePackage(weapons[equippedWeapon].data.damage, objectName, gameObject.name);
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
                netUI.CmdFeedMessage(dmgPck.hitter + " has killed " + objectName + "!");
                netUI.UpdateEntry(gameObject.name, 0, 1);
                netUI.UpdateEntry(dmgPck.hitterID, 1, 0);
            }
        }
    }

    [Command]
    void CmdSendPopup(string _message, string senderID, bool _sendToSender)
    {

    }
}
