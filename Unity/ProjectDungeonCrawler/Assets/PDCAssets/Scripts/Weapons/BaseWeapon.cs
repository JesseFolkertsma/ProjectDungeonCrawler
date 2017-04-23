using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.StatusEffects;
using PDC.Characters;

namespace PDC.Weapons
{
    public enum WeaponType
    {
        Fist = 0,
        Pistol = 1,
        Spear = 2,
    }

    public abstract class BaseWeapon : Pickup
    {
        public string weaponName = "New Weapon";
        public WeaponType type;
        public float damage;
        public Vector3 offset;
        [Header("Effects on hit")]
        public StatusEffect[] effects;
        public Hitbox hitbox;
        public bool attacking = false;

        public virtual void LightAttack(HumanoidCharacter hc)
        {
            if (attacking)
                return;
        }
        public virtual void HeavyAttack(HumanoidCharacter hc)
        {
            if (attacking)
                return;
        }

        public void Throw(HumanoidCharacter hc)
        {
            OnDrop();
            rb.AddForce(Camera.main.transform.forward * hc.throwForce);
            isThrown = true;
        }

        public void SetupWeapon()
        {
            hitbox = GetComponentInChildren<Hitbox>();
            rb = GetComponent<Rigidbody>();
        }

        public void EnableHitbox()
        {
            if (hitbox != null)
                hitbox.Enable();

            attacking = true;
        }
        public void DisableHitbox()
        {
            if (hitbox != null)
                hitbox.Disable();

            attacking = false;
        }

        public void OnDrop()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.parent = null;
            rb.isKinematic = false;
            pickupCol.SetActive(true);
        }

        public override void PickupItem(HumanoidCharacter hc)
        {
            if (hc.PickupWeapon(this))
            {
                rb.isKinematic = true;
                pickupCol.SetActive(false);
                canPickup = false;
                transform.localEulerAngles += offset;
                gameObject.layer = LayerMask.NameToLayer("PlayerArms");
                lightEffect.SetActive(false);
            }
        }
    }
}
