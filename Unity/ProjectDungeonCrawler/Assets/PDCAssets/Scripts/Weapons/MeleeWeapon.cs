using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class MeleeWeapon : Weapon
    {
        [Header("MeleeWeapon variables")]
        public Hitbox hitbox;

        bool mouseDown = false;
        bool waitingForInput = false;

        public override void OnStart()
        {
            base.OnStart();
            hitbox.SetupHitbox(this);
        }

        public override void Attack()
        {
            canAttack = false;
            anim.SetTrigger("Attack");
            GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(AttackAnimationEnd);
            GameManager.instance.CheckWhenAnimationTagEnds(anim, "Attack", newDelegate);
        }

        public void FollowUpAttack()
        {
            waitingForInput = false;
            anim.SetTrigger("Attack");
        }

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (!mouseDown)
            {
                mouseDown = true;
                if (canAttack)
                {
                    Attack();
                }
                else if (waitingForInput)
                {
                    FollowUpAttack();
                }
            }
        }

        void AttackAnimationEnd()
        {
            canAttack = true;
            waitingForInput = false;
        }

        public override void Fire1Up()
        {
            mouseDown = false;
        }

        public override void Fire2Hold()
        {

        }

        public override void Fire2Up()
        {

        }

        public void WaitForFollowup()
        {
            print("wait");
            waitingForInput = true;
        }

        public void EnableHitbox()
        {
            print("Enable");
            hitbox.Enable();
        }

        public void DisableHitbox()
        {
            print("Disable");
            hitbox.Disable();
            hitbox.ClearHits();
        }

        public virtual void HitboxHit(IHitable hit)
        {
            hit.GetHit(damage, EffectType.Normal, weaponEffects, transform.position);
        }
    }
}
