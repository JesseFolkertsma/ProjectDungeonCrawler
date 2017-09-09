using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Weapons
{
    public class EmptyWeapon : Weapon
    {

        public override void ThrowWeapon(Camera playercam, float strenght)
        {
            Destroy(gameObject);
        }

        public override void Attack()
        {

        }

        public override void Fire1Hold(Camera playercam, LayerMask mask)
        {

        }

        public override void Fire1Up()
        {

        }

        public override void Fire2Hold()
        {

        }

        public override void Fire2Up()
        {

        }

        public static EmptyWeapon GetNew()
        {
            GameObject newObject = new GameObject();
            newObject.name = "EmptyWeapon object";
            newObject.AddComponent<EmptyWeapon>();
            return newObject.GetComponent<EmptyWeapon>();
        }
    }
}
