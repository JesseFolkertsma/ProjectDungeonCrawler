using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponUtility {

    public static IHitable[] GetEnemiesInAttack(WeaponData weaponData, Transform camera)
    {
        if (weaponData == null)
        {
            Debug.LogError("You little diptard you need to give me some weapondata!");
            return new IHitable[0];
        }

        switch (weaponData.attackType)
        {
            case AttackType.Raycast:
                return WURaycast(weaponData, camera);
            case AttackType.Hitbox:
                return WUBoxCast(weaponData, camera);
        }
        return new IHitable[0];
    }

    static IHitable[] WUBoxCast(WeaponData wData, Transform cam)
    {
        return new IHitable[0];
    }

    static IHitable[] WURaycast(WeaponData wData, Transform cam)
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, wData.range, -LayerMask.NameToLayer("RemotePlayer")))
        {
            IHitable rayHit = hit.transform.gameObject.GetComponent<IHitable>();
            if (hit.transform.gameObject.GetComponent<IHitable>() != null)
            {
                IHitable[] rayhits = new IHitable[1];
                rayhits[0] = rayHit;
                return rayhits;
            }
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
