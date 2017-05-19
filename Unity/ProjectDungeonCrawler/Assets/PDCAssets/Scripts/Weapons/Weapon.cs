using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using PDC.Characters;

namespace PDC.Weapons
{
    public enum WeaponType
    {
        Revolver,
        Spear,
        Shotgun,
        Bow,
    }

    [System.Serializable]
    public abstract class Weapon : MonoBehaviour
    {
        public string weaponName = "New Weapon";
        public WeaponType type;
        public float damage = 20;
        public float throwDamage = 20;
        public StatusEffect[] weaponEffects;
        public int maxAmmo = 8;
        public int ammo = 8;
        public bool isEquipped;

        [HideInInspector] public int assignedSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        public GameObject physicsCol;
        [HideInInspector]
        public bool canAttack = true;

        public abstract void Fire1Hold(Camera playercam, LayerMask mask);
        public abstract void Fire1Up();
        public abstract void Fire2Hold();
        public abstract void Fire2Up();
        public abstract void Attack();

        private void Awake()
        {
            canAttack = true;
            rb = GetComponent<Rigidbody>();
            if(GetComponent<Animator>() != null)
                anim = GetComponent<Animator>();
        }

        public void Throw(Camera playercam)
        {
            transform.parent = null;
            rb.isKinematic = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            print("Collision boii");

            if (!isEquipped)
            {
                if (collision.transform.root.tag == "Player")
                {
                    collision.transform.root.GetComponent<PlayerCombat>().PickupWeapon(this);
                    print("Getting pickedup boiii");
                }
                else if (rb.velocity.magnitude > 1)
                {
                    IHitable iHit = collision.transform.GetComponent<IHitable>();
                    if (iHit != null)
                    {
                        iHit.GetHit(damage, EffectType.Normal);
                    }
                }
            }
        }
    }
}
