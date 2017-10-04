using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponUtility {

    public static IHitable[] ShootWithRaycast(AttackType attackType, WeaponData weaponData)
    {
        switch (attackType)
        {
            case AttackType.Raycast:
                return WURaycast(weaponData);
            case AttackType.Hitbox:
                return WUBoxCast(weaponData);
        }
        return new IHitable[0];
    }

    static IHitable[] WUBoxCast(WeaponData weaponData)
    {
        return new IHitable[0];
    }

    static IHitable[] WURaycast(WeaponData weaponData)
    {
        return new IHitable[0];
    }
}

public enum AttackType
{
    Raycast,
    Hitbox,
    Projectile,
};
