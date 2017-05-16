using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;

namespace PDC.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public string weaponName = "New Weapon";
        public float damage = 20;
        public StatusEffect[] weaponEffects;
    }
}
