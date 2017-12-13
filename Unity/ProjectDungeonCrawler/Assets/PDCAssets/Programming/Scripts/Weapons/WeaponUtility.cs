using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponUtility {

    public class IHitableHit
    {
        public IHitable iHit;
        public RaycastHit rayHit;

        public IHitableHit() { }
        public IHitableHit(IHitable _ihit, RaycastHit _rayhit)
        {
            iHit = _ihit;
            rayHit = _rayhit;
        }
    }

    public static IHitableHit GetEnemiesInAttack(WeaponData weaponData, Ray ray)
    {
        if (weaponData == null)
        {
            Debug.LogError("You need to give me weapondata!");
            return null;
        }

        switch (weaponData.attackType)
        {
            case AttackType.Raycast:
                return WURaycast(weaponData, ray);
            //case AttackType.Hitbox:
                // WUBoxCast(weaponData, camera);
        }
        return null;
    }

    static IHitableHit[] WUBoxCast(WeaponData wData, Transform cam)
    {
        return new IHitableHit[0];
    }

    static IHitableHit WURaycast(WeaponData wData, Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, wData.range, GameManager.instance.hitableLayers))
        {
            IHitableHit rayHit = new IHitableHit(hit.collider.transform.GetComponent<IHitable>(), hit);
            return rayHit;
        }
        return null;
    }
}

public enum AttackType
{
    Raycast,
    Hitbox,
    Projectile,
};
