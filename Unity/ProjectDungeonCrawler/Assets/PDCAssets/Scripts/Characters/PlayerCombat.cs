﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.UI;

namespace PDC.Characters
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCombat : BaseCharacter
    {
        public static PlayerCombat instance;
        PlayerController pc;

        //Public variables
        public List<Weapon> weapons = new List<Weapon>();
        public int availableSlots;
        public Transform weaponPos;
        public float throwStrenght = 700;

        //Private variables
        Transform weaponTrans;
        int equippedWeapon;

        //Hidden public variables
        [HideInInspector]
        public Animator weaponAnim;

        //Delegates
        public delegate void OnPlayerSpawn();
        public static event OnPlayerDeath onSpawnEvent;
        public delegate void OnPlayerDeath();
        public static event OnPlayerDeath onDeathEvent;
        public delegate void OnWeaponDataChange(List<Weapon> weapons, Weapon equipped);
        public OnWeaponDataChange onWeaponDataChange;
        public delegate void OnAmmoDataChange(Weapon equipped);
        public OnAmmoDataChange onAmmoDataChange;
        public delegate void OnTakeDamage(float newHP, float maxHP);
        public OnTakeDamage onTakeDamage;
        public delegate void OnConsumableChange();
        public OnConsumableChange onConsumableChange;

        public Weapon EquippedWeapon
        {
            get
            {
                if (equippedWeapon < weapons.Count && equippedWeapon != -1)
                {
                    if (weapons[equippedWeapon] != null)
                        return weapons[equippedWeapon];
                }
                return null;
            }
        }

        void Start()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            pc = GetComponent<PlayerController>();

            weaponAnim = weaponPos.GetComponent<Animator>();
            if(EquippedWeapon == null)
            {
                equippedWeapon = -1;
            }

            if (onSpawnEvent != null)
                onSpawnEvent();
        }

        void Update()
        {
            if (!isdead)
            {
                CheckInput();
                WeaponEffects();
            }
        }

        void WeaponEffects()
        {
            WeaponSway();
            WeaponAnimation();
        }

        void WeaponAnimation()
        {
            float walkF = pc.acc / pc.acceleration;
            if (!pc.grounded)
                walkF = Mathf.Lerp(walkF, 0, Time.deltaTime * 6);
            weaponAnim.SetFloat("Walk", walkF);
        }

        void WeaponSway()
        {
            if (weaponTrans)
            {
                Vector3 newPos = new Vector3(-pc.xInput / 7, -pc.rb.velocity.y / 20, 0);
                if (newPos.y > .1f)
                    newPos.y = .1f;
                else if (newPos.y < -.1f)
                    newPos.y = -.1f;
                weaponTrans.localPosition = Vector3.Lerp(weaponTrans.localPosition, newPos, Time.deltaTime * 2);
            }
        }

        void CheckInput()
        {
            if (EquippedWeapon != null)
            {
                if (Input.GetButton("Fire2"))
                {
                    RightMouse();
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    RightMouseUp();
                }
                if (Input.GetButton("Fire1"))
                {
                    LeftMouse();
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    LeftMouseUp();
                }
                if (Input.GetButtonDown("Throw"))
                {
                    Throw();
                }
            }

            for (int i = 0; i < availableSlots; i++)
            {
                if (Input.GetButton((i + 1).ToString()))
                {
                    EquipWeapon(i);
                }
            }
        }

        void LeftMouse()
        {
            EquippedWeapon.Fire1Hold(pc.playerCam, pc.playerLayer);
            if(onAmmoDataChange != null)
                onAmmoDataChange(EquippedWeapon);
        }

        void RightMouse()
        {
            EquippedWeapon.Fire2Hold();
        }

        void LeftMouseUp()
        {
            EquippedWeapon.Fire1Up();
        }

        void RightMouseUp()
        {
            EquippedWeapon.Fire2Up();
        }

        void Throw()
        {
            if (EquippedWeapon != null)
            {
                EquippedWeapon.anim.SetTrigger("Throw");
                Weapon.OnAnimationEnd onAnimEnd = new Weapon.OnAnimationEnd(ThrowWeapon);
                EquippedWeapon.CheckWhenAnimationTagEnds(EquippedWeapon.anim, "Throw", onAnimEnd);
            }
        }

        void ThrowWeapon()
        {
            //Reset weapon slot
            weaponTrans = null;
            EquippedWeapon.Throw(pc.playerCam, throwStrenght);
            weapons[EquippedWeapon.assignedSlot] = null;
            equippedWeapon = -1;

            if(onWeaponDataChange != null)
                onWeaponDataChange(weapons, EquippedWeapon);
        }

        public void EquipWeapon(int weapI)
        {
            //Return if the slot is already the equipped slot
            if (EquippedWeapon != null)
            {
                if (weapI == EquippedWeapon.assignedSlot)
                    return;
            }

            //Disable equipped
            if(EquippedWeapon != null)
            {
                EquippedWeapon.gameObject.SetActive(false);
            }

            //Check if chosen weapon exists
            if (weapI < weapons.Count)
            {
                if (weapons[weapI] != null)
                {
                    equippedWeapon = weapI;
                    EquippedWeapon.gameObject.SetActive(true);
                    weaponTrans = EquippedWeapon.transform;
                    weaponAnim.SetTrigger("Equip");
                }
                else
                {
                    equippedWeapon = -1;
                }
            }
            if (onWeaponDataChange != null)
                onWeaponDataChange(weapons, EquippedWeapon);
        }

        //Check for a free slot and return true if so
        public bool TryAssignWeapon(Weapon weap)
        {
            for (int i = 0; i < availableSlots; i++)
            {
                if (weapons.Count - 1 < i)
                {
                    weapons.Add(weap);
                    weap.assignedSlot = i;
                    return true;
                }
                else if (weapons[i] == null)
                {
                    weapons[i] = weap;
                    weap.assignedSlot = i;
                    return true;
                }
            }
            return false;
        }

        public void PickupWeapon(Weapon weap)
        {
            //Check for free slot
            if (TryAssignWeapon(weap))
            {
                //Setup variables
                weap.isEquipped = true;
                weap.rb.isKinematic = true;
                weap.transform.parent = weaponPos;
                weap.transform.localPosition = Vector3.zero;
                weap.transform.localRotation = Quaternion.identity;
                weap.SetLayerRecursively(weap.gameObject, "Equipped");
                weap.physicsCol.SetActive(false);
                weap.gameObject.SetActive(false);
                if(EquippedWeapon == null)
                {
                    EquipWeapon(weap.assignedSlot);
                }
                print("Assigned to slot: " + weap.assignedSlot);
                if (onWeaponDataChange != null)
                    onWeaponDataChange(weapons, EquippedWeapon);
            }
            else
            {
                print("No available slot!");
            }
        }
        
        public override void TakeDamage(float damage, EffectType damageType)
        {
            base.TakeDamage(damage, damageType);
            if (onTakeDamage != null)
                onTakeDamage(characterStats.currentHP, characterStats.MaxHP);
        }

        public override void Die()
        {
            if (!isdead)
            {
                isdead = true;
                pc.playerCam.transform.parent = null;
                pc.playerCam.gameObject.AddComponent<CapsuleCollider>();
                pc.playerCam.gameObject.AddComponent<Rigidbody>();
                pc.playerCam.GetComponent<Rigidbody>().AddForce(transform.forward);
                onDeathEvent();
            }
        }
    }
}
