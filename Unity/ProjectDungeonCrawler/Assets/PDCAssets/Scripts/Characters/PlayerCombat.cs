using System;
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
        public float throwStrenght = 700;

        //Private variables
        [SerializeField] Transform weaponPos;
        [SerializeField] Transform offSetObject;
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

            if (onSpawnEvent != null)
                onSpawnEvent();
            
            for (int i = 0; i < availableSlots; i++)
            {
                PickupWeapon(EmptyWeapon.GetNew());
            }
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
                Vector3 sway = new Vector3(-(Mathf.Clamp(Input.GetAxisRaw("Mouse X"), -1, 1) + pc.xInput) / 14, -pc.rb.velocity.y / 20, 0);
                Vector3 newPos = EquippedWeapon.weaponHolderPositionOffset + sway;
                if (newPos.y > .1f)
                    newPos.y = .1f;
                else if (newPos.y < -.1f)
                    newPos.y = -.1f;
                offSetObject.localPosition = Vector3.Lerp(offSetObject.localPosition, newPos, Time.deltaTime * 2);
            }
        }

        void CheckInput()
        {
            if (EquippedWeapon.GetType() != typeof(EmptyWeapon))
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

            //Check scrollwheel input
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

            if(scroll != 0)
            {
                //Check if scroll is negative or positive (cast to int)
                int equipThis = 0;
                if (scroll < 0)
                {
                    equipThis = EquippedWeapon.assignedSlot + 1;
                    if (equipThis > weapons.Count - 1)
                        equipThis = 0;
                }
                else if (scroll > 0)
                {
                    equipThis = EquippedWeapon.assignedSlot - 1;
                    if (equipThis < 0)
                        equipThis = weapons.Count - 1;
                }

                //Equip the right weapon
                EquipWeapon(equipThis);
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
            if (EquippedWeapon.GetType() != typeof(EmptyWeapon))
                EquippedWeapon.anim.SetTrigger("Throw");
        }

        public void ThrowWeapon()
        {
            //Reset weapon slot
            EquippedWeapon.ThrowWeapon(pc.playerCam, throwStrenght + throwStrenght * pc.acc);
            if (EquippedWeapon.GetType() != typeof(EmptyWeapon))
            {
                weaponTrans = null;
                RemoveWeapon(EquippedWeapon.assignedSlot);

                if (onWeaponDataChange != null)
                    onWeaponDataChange(weapons, EquippedWeapon);
            }
        }

        public void EquipWeapon(int weapI)
        {
            //Return if the slot is already the equipped slot
            if (weapI == EquippedWeapon.assignedSlot && EquippedWeapon.gameObject.activeInHierarchy)
                return;

            if (EquippedWeapon.GetType() != typeof(EmptyWeapon))
                EquippedWeapon.gameObject.SetActive(false);
            
            equippedWeapon = weapI;

            if (weapons[weapI].GetType() != typeof(EmptyWeapon))
            {
                offSetObject.localPosition = weapons[weapI].weaponHolderPositionOffset;
                offSetObject.localEulerAngles = weapons[weapI].weaponHolderRotationOffset;
                EquippedWeapon.gameObject.SetActive(true);
                weaponTrans = EquippedWeapon.transform;
                weapons[weapI].anim.SetTrigger("Pickup");
                weaponAnim.SetTrigger("Equip");
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
                else if (weap.GetType() == typeof(EmptyWeapon))
                {
                    if (weapons[i] == null)
                    {
                        weapons[i] = weap;
                        weap.assignedSlot = i;
                        return true;
                    }
                    continue;
                }
                else if (weapons[i].GetType() == typeof(EmptyWeapon))
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
                if (weap.GetType() != typeof(EmptyWeapon))
                {
                    weap.pc = this;
                    weap.rb.isKinematic = true;
                    weap.transform.parent = offSetObject;
                    weap.transform.localPosition = Vector3.zero;
                    weap.transform.localRotation = Quaternion.identity;
                    weap.SetLayerRecursively(weap.gameObject, "Equipped");
                    weap.physicsCol.SetActive(false);
                    weap.gameObject.SetActive(false);
                }
                else
                {
                    print("Assigning empty");
                }
                EquipWeapon(weap.assignedSlot);
                if (onWeaponDataChange != null)
                    onWeaponDataChange(weapons, EquippedWeapon);

                weap.OnPickup();
            }
            else
            {
                print("No available slot!");
            }
        }

        void RemoveWeapon(int slotToRemove)
        {
            weapons[slotToRemove] = null;
            PickupWeapon(EmptyWeapon.GetNew());
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
