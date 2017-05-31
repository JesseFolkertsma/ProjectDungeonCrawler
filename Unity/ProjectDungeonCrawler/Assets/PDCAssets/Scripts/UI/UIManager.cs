using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.StatusEffects;
using PDC.Characters;

namespace PDC.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public delegate void OnWeaponVisual(List<Weapon> weapons, Weapon equipped);
        public OnWeaponVisual onWeaponVisual;
        public delegate void OnAmmo(Sprite ammoSprite, int ammo);
        public OnAmmo onAmmo;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            PlayerController.onSpawnEvent += OnPlayerSpawn;
            PlayerController.onDeathEvent += OnPlayerDeath;
        }

        private void OnPlayerSpawn()
        {
            PlayerCombat.instance.onWeaponDataChange += UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange += UpdateAmmo;
            PlayerController.instance.onTakeDamage += UpdateHP;
        }

        private void OnPlayerDeath()
        {
            PlayerCombat.instance.onWeaponDataChange -= UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange -= UpdateAmmo;
        }

        void UpdateWeaponVisual(List<Weapon> weapons, Weapon equipped)
        {
            if(equipped != null)
                UpdateAmmo(equipped.weaponIcon, equipped.ammo);
        }

        void UpdateAmmo(Sprite ammoSprite, int ammo)
        {

        }

        void UpdateHP(float health, float maxHealth)
        {

        }

        void UpdateSouls(float souls, float maxSouls)
        {

        }
    }
}
