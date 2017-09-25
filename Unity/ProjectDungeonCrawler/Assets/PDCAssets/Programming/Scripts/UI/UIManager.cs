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
        CanvasReferences spawnedCanvas;

        public delegate void OnWeaponVisual(List<Weapon> weapons, Weapon equipped);
        public OnWeaponVisual onWeaponVisual;
        public delegate void OnAmmo(Weapon equipped);
        public OnAmmo onAmmo;

        float coins;
        public float coinsToAcuire = 500;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            PlayerCombat.onSpawnEvent += OnPlayerSpawn;
        }

        bool spawned = false;
        private void OnPlayerSpawn()
        {
            if (!spawned)
            {
                spawned = true;
                spawnedCanvas = Instantiate(canvasRef);
                for (int i = 0; i < GameManager.instance.gameData.availableSlots; i++)
                {
                    spawnedCanvas.SpawnNewSlot();
                }
                PlayerCombat.onDeathEvent += OnPlayerDeath;
                GameManager.instance.gameData.onWeaponDataChange += UpdateWeaponVisual;
                PlayerCombat.instance.onAmmoDataChange += spawnedCanvas.SetAmmoVisuals;
                GameManager.instance.gameData.onConsumableChange += UpdateConsumable;
                PlayerCombat.instance.onHPChange += spawnedCanvas.SetHp;
                PlayerCombat.instance.onGiveStatusEffect += spawnedCanvas.AddStatusEffect;
                SetupUI(PlayerCombat.instance);
            }
        }

        private void OnPlayerDeath()
        {
            spawned = false;
            PlayerCombat.onDeathEvent -= OnPlayerDeath;
            GameManager.instance.gameData.onWeaponDataChange -= UpdateWeaponVisual;
            PlayerCombat.instance.onAmmoDataChange -= spawnedCanvas.SetAmmoVisuals;
            GameManager.instance.gameData.onConsumableChange -= UpdateConsumable;
            PlayerCombat.instance.onHPChange -= spawnedCanvas.SetHp;
            PlayerCombat.instance.onGiveStatusEffect -= spawnedCanvas.AddStatusEffect;
        }

        void SetupUI(PlayerCombat pc)
        {
            UpdateWeaponVisual(GameManager.instance.gameData.weapons, GameManager.instance.gameData.EquippedWeapon);
            UpdateConsumable(GameManager.instance.gameData.consumables, 0, false);
            spawnedCanvas.SetHp(pc.characterStats.currentHP, pc.characterStats.MaxHP);
        }

        void UpdateWeaponVisual(List<Weapon> weapons, Weapon equipped)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                spawnedCanvas.SetWeaponSlot(i, weapons[i], (weapons[i] == equipped));
            }

            if(equipped != null)
                spawnedCanvas.SetAmmoVisuals(equipped);
            else
                spawnedCanvas.SetAmmoVisualState(false);
        }

        public void AddProgress()
        {
            coins++;
            spawnedCanvas.SetQuestProgression(coins, coinsToAcuire);
        }

        void UpdateConsumable(List<Consumable> consumables, int selected, bool playAnimation)
        {
            if (consumables.Count > 0)
            {
                if (playAnimation)
                    spawnedCanvas.PlayerConsumableAnimation(consumables[selected].icon);
                else
                    spawnedCanvas.SetConsumableVisual(consumables, selected);
            }
            else
            {
                spawnedCanvas.SetConsumableVisual(consumables, selected);
            }
        }
    }
}
