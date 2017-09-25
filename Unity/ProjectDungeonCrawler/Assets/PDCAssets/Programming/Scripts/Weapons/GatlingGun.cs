using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class GatlingGun : RangedGun
    {
        [Header("GatlingGun personal stats")]
        [Tooltip("Rounds per second")]
        public float fireRate = 3;

        float timer = 0;

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if(Time.time > timer)
            {
                if (ammo > 0)
                {
                    timer = Time.time + 1 / fireRate;
                    cam = playercam;
                    m = mask;
                    Attack();
                }
                else
                {
                    PlaySound(dryfireSound);
                }
            }
            anim.SetBool("Attacking", true);
        }

        public override void Attack()
        {
            if (cam != null)
            {
                ammo--;
                ShootVisuals();
                DamageRaycast();
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
    }
}
