using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.StatusEffects;
using PDC.Consumables;
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
            PlayerCombat.onSpawnEvent += OnPlayerSpawn;
            PlayerCombat.onDeathEvent += OnPlayerDeath;
        }

        private void OnPlayerSpawn()
        {
            canvasRef = Instantiate(canvasRef);
            for (int i = 0; i < PlayerCombat.instance.availableSlots; i++)
            {
                canvasRef.SpawnNewSlot();
            }
            PlayerCombat.instance.onWeaponDataChange += UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange += canvasRef.SetAmmoVisuals;
            PlayerCombat.instance.onConsumableChange += UpdateConsumable;
            PlayerCombat.instance.onHPChange += canvasRef.SetHp;
            PlayerCombat.instance.onGiveStatusEffect += canvasRef.AddStatusEffect;
        }

        private void OnPlayerDeath()
        {
            PlayerCombat.instance.onWeaponDataChange -= UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange -= canvasRef.SetAmmoVisuals;
            PlayerCombat.instance.onConsumableChange -= UpdateConsumable;
            PlayerCombat.instance.onHPChange -= canvasRef.SetHp;
            PlayerCombat.instance.onGiveStatusEffect -= canvasRef.AddStatusEffect;
        }

        void UpdateWeaponVisual(List<Weapon> weapons, Weapon equipped)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                canvasRef.SetWeaponSlot(i, weapons[i], (weapons[i] == equipped));
            }

            if(equipped != null)
                canvasRef.SetAmmoVisuals(equipped);
            else
                canvasRef.SetAmmoVisualState(false);
        }

        void UpdateConsumable(List<Consumable> consumables, int selected, bool playAnimation)
        {
            if (playAnimation)
                canvasRef.PlayerConsumableAnimation(consumables[selected].icon);
            else
                canvasRef.SetConsumableVisual(consumables[selected].icon);
        }

        void AddStatusEffect(OngoingEffect effect)
        {

        }

        void UpdateSouls(float souls, float maxSouls)
        {

        }
    }
}
