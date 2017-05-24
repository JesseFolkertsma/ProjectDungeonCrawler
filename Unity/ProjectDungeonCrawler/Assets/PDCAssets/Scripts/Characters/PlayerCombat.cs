using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.UI;

namespace PDC.Characters
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCombat : MonoBehaviour
    {
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

        void Awake()
        {
            pc = GetComponent<PlayerController>();

            weaponAnim = weaponPos.GetComponent<Animator>();
            if(EquippedWeapon == null)
            {
                equippedWeapon = -1;
            }
        }

        void Update()
        {
            if (!pc.isdead)
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
            UIManager.instance.UpdateAmmo(weapons[equippedWeapon].ammoIcon, weapons[equippedWeapon].ammo);
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
            StartCoroutine(CheckWhenThrowEnds());
        }
        
        IEnumerator CheckWhenThrowEnds()
        {
            weaponAnim.SetTrigger("Throw");
            yield return new WaitForEndOfFrame();
            while (weaponAnim.GetCurrentAnimatorStateInfo(0).IsName("Throw"))
            {
                yield return new WaitForEndOfFrame();
            }
            ThrowWeapon();
        }

        void ThrowWeapon()
        {
            weaponTrans = null;
            EquippedWeapon.Throw(pc.playerCam, throwStrenght);
            weapons[EquippedWeapon.assignedSlot] = null;
            equippedWeapon = -1;
            UIManager.instance.UpdateWeaponVisual(weapons, EquippedWeapon);
        }

        public void EquipWeapon(int weapI)
        {
            if(EquippedWeapon != null)
            {
                EquippedWeapon.gameObject.SetActive(false);
            }
            if (weapI < weapons.Count)
            {
                if (weapons[weapI] != null)
                {
                    equippedWeapon = weapI;
                    EquippedWeapon.gameObject.SetActive(true);
                    weaponTrans = EquippedWeapon.transform;
                    weaponAnim.SetTrigger("Equip");
                }
            }
            UIManager.instance.UpdateWeaponVisual(weapons, EquippedWeapon);
        }

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
            if (TryAssignWeapon(weap))
            {
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
                UIManager.instance.UpdateWeaponVisual(weapons, EquippedWeapon);
            }
            else
            {
                print("CAnt pickup boii");
            }
        }
    }
}
