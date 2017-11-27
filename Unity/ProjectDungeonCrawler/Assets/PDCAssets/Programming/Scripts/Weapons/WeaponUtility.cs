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

    public static IHitableHit GetEnemiesInAttack(WeaponData weaponData, Transform camera)
    {
        if (weaponData == null)
        {
            Debug.LogError("You little diptard you need to give me some weapondata!");
            return new IHitableHit();
        }

        switch (weaponData.attackType)
        {
            case AttackType.Raycast:
                return WURaycast(weaponData, camera);
            //case AttackType.Hitbox:
                // WUBoxCast(weaponData, camera);
        }
        return new IHitableHit();
    }

    static IHitableHit[] WUBoxCast(WeaponData wData, Transform cam)
    {
        return new IHitableHit[0];
    }

    static IHitableHit WURaycast(WeaponData wData, Transform cam)
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, wData.range))
        {
            IHitableHit rayHit = new IHitableHit(hit.collider.transform.GetComponent<IHitable>(), hit);
            return rayHit;
        }
        return new IHitableHit();
    }
}

public enum AttackType
{
    Raycast,
    Hitbox,
    Projectile,
};
