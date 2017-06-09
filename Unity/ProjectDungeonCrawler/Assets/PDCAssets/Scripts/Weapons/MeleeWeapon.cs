using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class MeleeWeapon : Weapon
    {
        [Header("MeleeWeapon variables")]
        public float range = 5f;

        bool mouseDown = false;
        bool waitingForInput = false;

        Camera cam;
        LayerMask lMask;
        MeleeAttackData attackData;

        public void SetupAttackData(MeleeAttackData data)
        {
            attackData = data;
        }

        public override void Attack()
        {
            canAttack = false;
            anim.SetTrigger("Attack");
            GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(AttackAnimationEnd);
            GameManager.instance.CheckWhenAnimationTagEnds(anim, "Attack", newDelegate);
        }

        public void AttackBox()
        {
            print("Box");
            Vector3 boxPos = cam.transform.position + (cam.transform.forward * (range / 2));
            Vector3 boxSize = new Vector3(0, 0, range);
            if(attackData.attackType == AttackType.Horizontal)
            {
                boxSize.x = 3;
                boxSize.y = attackData.attackWidthOrHeight;
            }
            else
            {
                boxSize.x = attackData.attackWidthOrHeight;
                boxSize.y = 3;
            }
            Collider[] hits = Physics.OverlapBox(boxPos, boxSize / 2 , cam.transform.rotation, lMask);
            foreach(Collider col in hits)
            {
                print("I hit: "  + col.name);
                IHitable iHit = col.GetComponent<IHitable>();
                if (iHit != null)
                {
                    Vector3 enemyDir = (iHit.ObjectCenter - cam.transform.position).normalized;
                    print(Vector3.Angle(enemyDir, cam.transform.forward));
                    if(Vector3.Angle(enemyDir, (cam.transform.forward + attackData.angleDirectionOffset).normalized) < attackData.hitAngle)
                    {
                        print(col.name + " is in Angle!");
                        RaycastHit rHit;
                        if (Physics.Raycast(cam.transform.position, enemyDir, out rHit, range, lMask))
                        {
                            if (rHit.collider == col)
                            {
                                foreach (IHitable h in col.GetComponents<IHitable>())
                                    h.GetHit(damage, EffectType.Normal, weaponEffects, cam.transform.position);
                            }
                        }
                    }
                }
            }
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
                cam = playercam;
                lMask = mask;
                //mouseDown = true;
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
    }
}
