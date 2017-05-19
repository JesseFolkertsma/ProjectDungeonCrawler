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
        public float attackRate = 1;
        public Transform gunEnd;
        public GameObject muzzleFlash;

        float cd;

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (cd < Time.time)
            {
                cd = Time.time + 1 / attackRate;
                RaycastHit hit;
                if (Physics.Raycast(playercam.transform.position, playercam.transform.forward, out hit, range, mask))
                {
                    IHitable[] hits = hit.transform.root.GetComponents<IHitable>();
                    foreach (IHitable h in hits)
                    {
                        h.GetHit(damage, EffectType.Normal);
                    }
                }
                anim.SetTrigger("Attack");
                print("pew");
            }
        }

        public override void Fire2Hold()
        {
            throw new NotImplementedException();
        }
    }
}
