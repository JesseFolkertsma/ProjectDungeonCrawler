using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    [System.Serializable]
    public class Revolver : RangedGun
    {
        bool buttonDown;

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (canAttack && !buttonDown)
            {
                buttonDown = true;
                if (ammo > 0)
                {
                    cam = playercam;
                    m = mask;
                    if (anim != null)
                    {
                        canAttack = false;
                        anim.SetTrigger("Attack");
                    }
                    else Debug.LogError(gameObject.name + "'s Animator variable is not setup dipnugget!");
                }
                else
                {
                    DryFire();
                }
            }
        }

        public override void DryFire()
        {
            base.DryFire();
            GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(AttackAnimationEnd);
            GameManager.instance.CheckWhenAnimationTagEnds(anim, "Attack", newDelegate);
        }

        public override void Attack()
        {
            if (cam != null)
            {
                ammo--;
                ShootVisuals();
                DamageRaycast();
                GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(AttackAnimationEnd);
                GameManager.instance.CheckWhenAnimationTagEnds(anim, "Attack", newDelegate);
            }
            else
            {
                Debug.LogError(gameObject.name + "'s cam variable not setup!");
            }
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
    }
}
