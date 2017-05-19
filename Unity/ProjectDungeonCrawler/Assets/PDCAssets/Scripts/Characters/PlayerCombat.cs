using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;

namespace PDC.Characters
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCombat : MonoBehaviour
    {
        PlayerController pc;

        //Public variables
        public Weapon equippedWeapon;
        public List<Weapon> weapons = new List<Weapon>();
        public int availableSlots;
        public Transform weaponPos;

        //Private variables
        Transform weaponTrans;

        //Hidden public variables
        [HideInInspector]
        public Animator weaponAnim;

        void Awake()
        {
            pc = GetComponent<PlayerController>();

            weaponAnim = weaponPos.GetComponent<Animator>();
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
            if (equippedWeapon != null)
            {
                if (Input.GetButton("Fire2"))
                {
                    equippedWeapon.Fire2Hold();
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    equippedWeapon.Fire2Up();
                }
                if (Input.GetButton("Fire1"))
                {
                    equippedWeapon.Fire1Hold(pc.playerCam, pc.playerLayer);
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    equippedWeapon.Fire1Up();
                }
                if (Input.GetButtonDown("Throw"))
                {
                    equippedWeapon.Throw(pc.playerCam);
                    weapons[equippedWeapon.assignedSlot] = null;
                    equippedWeapon = null;
                }
            }
        }

        public void EquipWeapon(int weapI)
        {
            if(weapons[weapI] != null)
            {
                equippedWeapon = weapons[weapI];
            }
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
                weap.rb.isKinematic = true;
                weap.transform.parent = weaponPos;
                weaponTrans = weap.transform;
                weap.transform.localPosition = Vector3.zero;
                weap.transform.localRotation = Quaternion.identity;
                weap.gameObject.layer = LayerMask.NameToLayer("Equipped");
                weap.physicsCol.SetActive(false);
                if(equippedWeapon == null)
                {
                    EquipWeapon(weap.assignedSlot);
                }
                print("Assigned to slot: " + weap.assignedSlot);
            }
            else
            {
                print("CAnt pickup boii");
            }
        }
    }
}
