using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.Weapons;
using PDC.StatusEffects;
using PDC.Consumables;

namespace PDC.Saving
{
    [System.Serializable]
    public class PlayerData
    {
        public Stats playerStats = new Stats();
        public List<Weapon> weapons = new List<Weapon>();
        public List<int> weaponsByID = new List<int>();
        public int availableSlots;
        public List<Consumable> consumables;
        public float throwStrenght = 700f;
        public int equippedWeapon;
        public int selectedConsumable;

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

        //Delegates
        public delegate void OnWeaponDataChange(List<Weapon> weapons, Weapon equipped);
        public OnWeaponDataChange onWeaponDataChange;
        public delegate void OnConsumableChange(List<Consumable> consumables, int selectedConsumable, bool playAnimation);
        public OnConsumableChange onConsumableChange;

        public void ConvertWeaponsToID()
        {
            weaponsByID = new List<int>();
            foreach(Weapon w in weapons)
            {
                weaponsByID.Add(w.weaponID);
            }
        }

        public void AddConsumable(Consumable cons)
        {
            consumables.Add(cons);
            if (onConsumableChange != null)
                onConsumableChange(consumables, selectedConsumable, false);
        }

        public void RemoveConsumable(Consumable cons)
        {
            if (consumables.Contains(cons))
                consumables.Remove(cons);
            if (onConsumableChange != null)
                onConsumableChange(consumables, selectedConsumable, false);
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

        public bool TryAssignWeapon(int weap)
        {
            for (int i = 0; i < availableSlots; i++)
            {
                if (weaponsByID.Count - 1 < i)
                {
                    weaponsByID.Add(weap);
                    return true;
                }
                else if (weap == 0)
                {
                    if (weapons[i] == null)
                    {
                        weaponsByID[i] = weap;
                        return true;
                    }
                    continue;
                }
                else if (weaponsByID[i] == 0)
                {
                    weaponsByID[i] = weap;
                    return true;
                }
            }
            return false;
        }
    }
}
