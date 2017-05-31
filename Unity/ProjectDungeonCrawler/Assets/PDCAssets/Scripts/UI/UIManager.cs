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

        public CanvasReferences canvasRef;

        public delegate void OnWeaponVisual(List<Weapon> weapons, Weapon equipped);
        public OnWeaponVisual onWeaponVisual;
        public delegate void OnAmmo(Weapon equipped);
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
            canvasRef = Instantiate(canvasRef);
        }

        private void OnPlayerDeath()
        {
            PlayerCombat.instance.onWeaponDataChange -= UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange -= UpdateAmmo;
        }

        void UpdateWeaponVisual(List<Weapon> weapons, Weapon equipped)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                canvasRef.SetWeaponSlot(i, weapons[i], (weapons[i] == equipped));
            }

            if(equipped != null)
                UpdateAmmo(equipped);
        }

        void UpdateAmmo(Weapon equippedWeapon)
        {
            canvasRef.SetAmmoVisuals(equippedWeapon);
        }

        void UpdateHP(float health, float maxHealth)
        {
            canvasRef.SetHp(health, maxHealth);
        }

        void UpdateSouls(float souls, float maxSouls)
        {

        }
    }
}
