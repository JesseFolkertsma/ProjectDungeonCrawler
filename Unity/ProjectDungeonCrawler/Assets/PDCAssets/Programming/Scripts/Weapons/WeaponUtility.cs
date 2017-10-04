using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponUtility {

    public static IHitable[] ShootWithRaycast(AttackType attackType, EquippedWeapon weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogError("You little diptard you need to give me some weapondata!");
            return new IHitable[0];
        }

        switch (attackType)
        {
            case AttackType.Raycast:
                return WURaycast(weaponData);
            case AttackType.Hitbox:
                return WUBoxCast(weaponData);
        }
        return new IHitable[0];
    }

    static IHitable[] WUBoxCast(EquippedWeapon weaponData)
    {
        return new IHitable[0];
    }

    static IHitable[] WURaycast(EquippedWeapon weaponData)
    {
        RaycastHit hit;
        if(Physics.Raycast(weaponData.visual.gunEnd.position, weaponData.visual.gunEnd.forward, out hit, weaponData.instance.stats.range))
        {

        }
        return new IHitable[0];
    }
}

public enum AttackType
{
    Raycast,
    Hitbox,
    Projectile,
};
