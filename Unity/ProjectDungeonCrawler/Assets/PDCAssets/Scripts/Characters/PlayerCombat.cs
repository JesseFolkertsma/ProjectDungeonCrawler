using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.UI;
using PDC.Consumables;
using PDC.StatusEffects;
using PDC.Saving;

namespace PDC.Characters
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCombat : BaseCharacter, IHitable
    {
        public static PlayerCombat instance;
        PlayerController pc;

        //Private variables
        PlayerData data;
        [SerializeField] Transform weaponPos;
        [SerializeField] Transform offSetObject;
        Transform weaponTrans;
        bool setup = false;

        //Hidden public variables
        [HideInInspector]
        public Animator weaponAnim;

        //Delegates
        public delegate void OnPlayerSpawn();
        public static event OnPlayerDeath onSpawnEvent;
        public delegate void OnPlayerDeath();
        public static event OnPlayerDeath onDeathEvent;
        public delegate void OnAmmoDataChange(Weapon equipped);
        public OnAmmoDataChange onAmmoDataChange;
        public delegate void OnHPChange(float newHP, float maxHP);
        public OnHPChange onHPChange;
        public delegate void OnGiveStatusEffect(OngoingEffect effect);
        public OnGiveStatusEffect onGiveStatusEffect;

        public Vector3 ObjectCenter
        {
            get
            {
                return transform.position + Vector3.up;
            }
        }

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            pc = GetComponent<PlayerController>();

            weaponAnim = weaponPos.GetComponent<Animator>();

            data = GameManager.instance.gameData;
            
            for (int i = 0; i < data.availableSlots; i++)
            {
                PickupWeapon(EmptyWeapon.GetNew());
            }
            GatherWeaponData();

            if (onSpawnEvent != null)
                onSpawnEvent();

            GameManager.instance.onSceneExit += OnSceneExit;
            setup = true;
        }

        void OnSceneExit()
        {
            if (onDeathEvent != null) 
                onDeathEvent();

            data.ConvertWeaponsToID();
        }

        void GatherWeaponData()
        {
            foreach(int i in data.weaponsByID)
            {
                GameObject go = Instantiate(WeaponDatabase.instace.GetWeaponByID(i));
                Weapon wep = go.GetComponent<Weapon>();
                PickupWeapon(wep);
            }
        }

        void Update()
        {
            if (!isdead && setup)
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
                Vector3 newPos = data.EquippedWeapon.weaponHolderPositionOffset + sway;
                if (newPos.y > .1f)
                    newPos.y = .1f;
                else if (newPos.y < -.1f)
                    newPos.y = -.1f;
                offSetObject.localPosition = Vector3.Lerp(offSetObject.localPosition, newPos, Time.deltaTime * 2);
            }
        }

        void CheckInput()
        {
            if (Input.GetButtonDown("Save"))
            {
                //GameManager.instance.GatherGameData();
            }
            if (Input.GetButtonDown("MainMenu"))
            {
                onDeathEvent();
                UnityEngine.SceneManagement.SceneManager.LoadScene("TestMain");
            }

            
            if (data.EquippedWeapon.GetType() != typeof(EmptyWeapon))
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
                if (onAmmoDataChange != null)
                    onAmmoDataChange(data.EquippedWeapon);
            }

            //Check scrollwheel input
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

            if(scroll != 0)
            {
                //Check if scroll is negative or positive (cast to int)
                int equipThis = 0;
                if (scroll < 0)
                {
                    equipThis = data.EquippedWeapon.assignedSlot + 1;
                    if (equipThis > data.weapons.Count - 1)
                        equipThis = 0;
                }
                else if (scroll > 0)
                {
                    equipThis = data.EquippedWeapon.assignedSlot - 1;
                    if (equipThis < 0)
                        equipThis = data.weapons.Count - 1;
                }

                //Equip the right weapon
                EquipWeapon(equipThis);
            }

            for (int i = 0; i < data.availableSlots; i++)
            {
                if (Input.GetButton((i + 1).ToString()))
                {
                    EquipWeapon(i);
                }
            }

            //Consumables inputs
            if (data.consumables.Count > 0)
            {
                if (Input.GetButtonDown("Consume"))
                {
                    Consume();
                }

                if (Input.GetButtonDown("Next consumable"))
                {
                    NextConsumable();
                }
            }
        }

        void LeftMouse()
        {
            data.EquippedWeapon.Fire1Hold(pc.playerCam, pc.playerLayer);
        }

        void RightMouse()
        {
            data.EquippedWeapon.Fire2Hold();
        }

        void LeftMouseUp()
        {
            data.EquippedWeapon.Fire1Up();
        }

        void RightMouseUp()
        {
            data.EquippedWeapon.Fire2Up();
        }

        void Throw()
        {
            if (data.EquippedWeapon.GetType() != typeof(EmptyWeapon))
                data.EquippedWeapon.anim.SetTrigger("Throw");
        }

        public void ThrowWeapon()
        {
            //Reset weapon slot
            data.EquippedWeapon.ThrowWeapon(pc.playerCam, data.throwStrenght);
            if (data.EquippedWeapon.GetType() != typeof(EmptyWeapon))
            {
                weaponTrans = null;
                RemoveWeapon(data.EquippedWeapon.assignedSlot);

                if (data.onWeaponDataChange != null)
                    data.onWeaponDataChange(data.weapons, data.EquippedWeapon);
            }
        }

        public void EquipWeapon(int weapI)
        {
            //Return if the slot is already the equipped slot
            if (weapI == data.EquippedWeapon.assignedSlot && data.EquippedWeapon.gameObject.activeInHierarchy)
                return;

            if (data.EquippedWeapon.GetType() != typeof(EmptyWeapon))
                data.EquippedWeapon.gameObject.SetActive(false);

            data.equippedWeapon = weapI;

            if (data.weapons[weapI].GetType() != typeof(EmptyWeapon))
            {
                offSetObject.localPosition = data.weapons[weapI].weaponHolderPositionOffset;
                offSetObject.localEulerAngles = data.weapons[weapI].weaponHolderRotationOffset;
                data.EquippedWeapon.gameObject.SetActive(true);
                weaponTrans = data.EquippedWeapon.transform;
                data.weapons[weapI].anim.SetTrigger("Pickup");
                weaponAnim.SetTrigger("Equip");
            }

            if (data.onWeaponDataChange != null)
                data.onWeaponDataChange(data.weapons, data.EquippedWeapon);
        }

        public void PickupConsumable(Consumable cons)
        {
            data.AddConsumable(cons);
        }

        public void RemoveConsumable(Consumable cons)
        {
            data.RemoveConsumable(cons);
        }

        void Consume()
        {
            data.consumables[data.selectedConsumable].Use(this);
            RemoveConsumable(data.consumables[data.selectedConsumable]);
        }

        void NextConsumable()
        {
            data.selectedConsumable++;
            if (data.selectedConsumable > data.consumables.Count - 1)
                data.selectedConsumable = 0;

            if (data.onConsumableChange != null)
                data.onConsumableChange(data.consumables, data.selectedConsumable, true);
        }

        public void PickupWeapon(Weapon weap)
        {
            //Check for free slot
            if (data.TryAssignWeapon(weap))
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
                if (data.onWeaponDataChange != null)
                    data.onWeaponDataChange(data.weapons, data.EquippedWeapon);

                weap.OnPickup();
            }
            else
            {
                print("No available slot!");
            }
        }

        void RemoveWeapon(int slotToRemove)
        {
            data.weapons[slotToRemove] = null;
            PickupWeapon(EmptyWeapon.GetNew());
        }

        public override void GiveStatusEffect(OngoingEffect effect)
        {
            base.GiveStatusEffect(effect);
            if (onGiveStatusEffect != null)
                onGiveStatusEffect(effect);
        }

        public override void TakeDamage(float damage, EffectType damageType)
        {
            base.TakeDamage(damage, damageType);
            if (onHPChange != null)
                onHPChange(characterStats.currentHP, characterStats.MaxHP);
        }

        public override void Heal(float hp)
        {
            base.Heal(hp);
            if (onHPChange != null)
                onHPChange(characterStats.currentHP, characterStats.MaxHP);
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
                data.EquippedWeapon.gameObject.SetActive(false);
                if(onDeathEvent != null)
                    onDeathEvent();
            }
        }

        public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
        {
            TakeDamage(damage, hitType);
            foreach(StatusEffect se in effects)
                se.AddEffect(this);
        }
    }
}
