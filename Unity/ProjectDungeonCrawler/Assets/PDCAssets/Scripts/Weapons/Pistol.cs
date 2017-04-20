using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PDC.Weapons
{
    public class Pistol : BaseWeapon
    {
        Transform pistolEnd;

        public override void Attack(PlayerController pc)
        {
            pc.anim.SetTrigger("LightAttack");
        }

        public override void Pickup()
        {
            throw new NotImplementedException();
        }

        public override void Throw()
        {
            throw new NotImplementedException();
        }
    }
}
