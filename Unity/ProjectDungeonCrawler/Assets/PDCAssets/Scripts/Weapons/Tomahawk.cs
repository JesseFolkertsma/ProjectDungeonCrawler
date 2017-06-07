using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class Tomahawk : Weapon
    {
        bool mouseDown = false;

        public override void Attack()
        {
            canAttack = false;
            anim.SetTrigger("Attack");
            GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(AttackAnimationEnd);
            GameManager.instance.CheckWhenAnimationTagEnds(anim, "Attack", newDelegate);
        }

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {
            if (!mouseDown && canAttack)
            {
                mouseDown = true;
                Attack();
            }
        }

        void AttackAnimationEnd()
        {
            canAttack = true;
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
    }
}
