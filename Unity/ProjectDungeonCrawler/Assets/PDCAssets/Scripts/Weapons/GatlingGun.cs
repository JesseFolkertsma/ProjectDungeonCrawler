using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class GatlingGun : Weapon
    {
        public float range = 100;
        [Tooltip("Rounds per second")]
        public float fireRate = 3;
        public Transform gunEnd;
        public GameObject muzzleFlash;
        public int ammoUsePerShot = 1;

        float timer = 0;
        Camera cam;
        LayerMask m;

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if(ammo > 0 && Time.time > timer)
            {
                timer = Time.time + 1 / fireRate;
                cam = playercam;
                m = mask;
                Attack();
            }
            anim.SetBool("Attacking", true);
        }

        public override void Attack()
        {
            if (cam != null)
            {
                ammo--;
                ShootVisuals();
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, m))
                {
                    IHitable iHit = hit.transform.GetComponent<IHitable>();
                    if (iHit != null)
                        iHit.GetHit(damage, EffectType.Normal, weaponEffects, cam.transform.position);
                }
            }
            else
            {
                Debug.LogError(gameObject.name + "'s cam variable not setup!");
            }
        }

        public override void Fire1Up()
        {
            anim.SetBool("Attacking", false);
        }

        public override void Fire2Hold()
        {

        }

        public override void Fire2Up()
        {

        }

        void ShootVisuals()
        {
            Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
        }
    }
}
