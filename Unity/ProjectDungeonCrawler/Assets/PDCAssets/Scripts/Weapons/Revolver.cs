﻿using System;
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
        bool buttonDown;

        Camera cam;
        LayerMask m;

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (canAttack && ammo > 0 && !buttonDown)
            {
                buttonDown = true;
                cam = playercam;
                m = mask;
                if (anim != null)
                {
                    canAttack = false;
                    anim.SetTrigger("Attack");
                    Attack();
                }
                else Debug.LogError(gameObject.name + "'s Animator variable is not setup dipnugget!");
            }
        }

        public override void Attack()
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
            OnAnimationEnd newDelegate = new OnAnimationEnd(AttackAnimationEnd);
            CheckWhenAnimationEnds(anim, "Attack", newDelegate);
        }

        public override void Fire1Up()
        {
            buttonDown = false;
        }

        public override void Fire2Hold()
        {

        }

        public override void Fire2Up()
        {

        }

        public void AttackAnimationEnd()
        {
            canAttack = true;
        }

        void ShootVisuals()
        {
            Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);
        }
    }
}
