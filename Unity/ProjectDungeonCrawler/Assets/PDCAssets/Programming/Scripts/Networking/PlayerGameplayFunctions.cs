using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGameplayFunctions : NetworkBehaviour {

    Player p;
    NWPlayerCombat combat;
    NetworkedController controller;

    [ClientRpc]
    void RpcGetFX()
    {
        //weaponVisuals[equippedWeapon].RemotePlayerEffectVisuals();
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
        //if (isLocalPlayer) return;
        //if (weaponVisuals != null) return;

        //weaponVisuals = new List<GunVisuals>();
        //GameObject wepGO = WeaponDatabase.instance.GetWeaponPrefab(inv.weapons[equippedWeapon]);
        //wepGO = Instantiate(wepGO, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        //GunVisuals wv = wepGO.GetComponent<GunVisuals>();
        //weaponVisuals.Add(wv);
        //if (equippedWeapon < 0 || equippedWeapon > inv.availableSlots - 1)
        //    equippedWeapon = 0;
    }

    [Command]
    void CmdDamageServer(NetworkPackages.DamagePackage dmgPck, NetworkInstanceId objectID)
    {
        NetworkServer.FindLocalObject(objectID).GetComponent<IHitable>().RpcGetHit(dmgPck);
    }

    [Command]
    void CmdDamageClient(string playerID, NetworkPackages.DamagePackage dmgPck)
    {
        //PlayerManager.GetPlayer(playerID).pCombat.RpcGetHit(dmgPck);
    }

    [ClientRpc]
    public void RpcGetHit(NetworkPackages.DamagePackage dmgPck)
    {
        //if (isDead)
        //    return;

        //testHP -= dmgPck.damage;
        //print("Is me " + playerName + "! And i hit hit with " + dmgPck.damage.ToString() + " damage by " + dmgPck.hitter + "! I now have " + testHP.ToString() + " health.");
        //if (isLocalPlayer)
        //    hud.UpdateHealth(testHP, 100);
        //if (testHP <= 0)
        //{
        //    Die();
        //    if (isLocalPlayer)
        //        CmdSendPopup(dmgPck.hitter + " has killed " + objectName, objectID, false);
        //}
    }
}
