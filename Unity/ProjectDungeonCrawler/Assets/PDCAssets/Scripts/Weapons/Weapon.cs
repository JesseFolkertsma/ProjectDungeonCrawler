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
        public int ammo = 8;
        public bool isEquipped;

        public int assignedSlot;
        public Rigidbody rb;
        public GameObject physicsCol;

        public abstract void Fire1Hold(Camera playercam, LayerMask mask);
        public abstract void Fire2Hold();

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Throw(Camera playercam)
        {
            transform.parent = null;
            rb.isKinematic = false;
        }

        public void Recoil(float str)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -str);
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
                    IHitable[] hits = collision.transform.root.GetComponentsInChildren<IHitable>();
                    foreach (IHitable h in hits)
                    {
                        h.GetHit(throwDamage, EffectType.Normal);
                    }
                }
            }
        }
    }
}
