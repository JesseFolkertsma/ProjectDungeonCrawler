using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    [System.Serializable]
    public class Revolver : Weapon
    {
        public float range = 100;
        public Transform gunEnd;
        public GameObject muzzleFlash;
        public int ammoUsePerShot = 1;
        
        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (canAttack && ammo > 0)
            {
                canAttack = false;
                ammo--;
                ShootVisuals();
                RaycastHit hit;
                if (Physics.Raycast(playercam.transform.position, playercam.transform.forward, out hit, range, mask))
                {
                    IHitable iHit = hit.transform.GetComponent<IHitable>();
                    if (iHit != null)
                    {
                        iHit.GetHit(damage, EffectType.Normal);
                    }
                }
                CheckIfAnimationEnd("Attack");
            }
        }

        void ShootVisuals()
        {
            Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
            if(anim)
                anim.SetTrigger("Attack");
        }

        public override void Fire2Hold()
        {
            throw new NotImplementedException();
        }
    }
}
