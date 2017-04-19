using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.StatusEffects;

namespace PDC.Weapons
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public string weaponName = "New Weapon";
        public float damage;
        [Header("Effects on hit")]
        public StatusEffect[] effects;

        public abstract void Attack(PlayerController pc);
        public abstract void Throw();
        public abstract void Pickup();
    }
}
