﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDC.Weapons;
using PDC.StatusEffects;

namespace PDC.UI
{
    public class CanvasReferences : MonoBehaviour
    {
        [Header("Canvas References")]
        [SerializeField] Image hpImage;
        [SerializeField] Image caveProgressionImage;
        [SerializeField] Image questProgressionImage;
        [SerializeField] Transform questLog;
        [SerializeField] List<Text> quests = new List<Text>();
        [SerializeField] Image ammoImage;
        [SerializeField] Text ammoText;
        [SerializeField] Transform weaponSlotHolder;
        [SerializeField] List<UIWeaponSlot> weaponSlots = new List<UIWeaponSlot>();
        [SerializeField] Image consumableImage;
        [SerializeField] Animator consumableAnim;
        [SerializeField] Transform statusEffectsHolder;

        [Header("Standalone Objects")]
        [SerializeField] GameObject newQuestObject;
        [SerializeField] GameObject newWeaponSlot;
        [SerializeField] GameObject newStatusEffect;
        [SerializeField] AudioClip swapWeaponSound;

        //Private variables
        AudioSource audioS;

        private void Awake()
        {
            audioS = GetComponent<AudioSource>();
        }

        public void SetHp(float currentHP, float maxHp)
        {
            if(currentHP < 0)
                hpImage.fillAmount = 0;
            else
                hpImage.fillAmount = currentHP / maxHp;
        }

        bool popped = false;
        public void SetQuestProgression(float coins, float maxCoins)
        {
            questProgressionImage.fillAmount = coins / maxCoins;
            if (questProgressionImage.fillAmount >= 1 && !popped)
            {
                questProgressionImage.fillAmount = 1;
                popped = true;
                GamePopup.instance.DisplayPopup("Enough gold obtained, good job!");
                GameManager.instance.vuileviezeint++;
            }
        }

        bool popped2 = false;
        public void SetCaveProgression(float kills, float enemies)
        {
            caveProgressionImage.fillAmount = kills / enemies;
            if (caveProgressionImage.fillAmount >= 1 && !popped)
            {
                caveProgressionImage.fillAmount = 1;
                popped2 = true;
                GamePopup.instance.DisplayPopup("All enemies killed! Press tab to leave.");
                GameManager.instance.vuileviezeint++;
            }
        }

        public void SetWeaponSlot(int slotToSet, Weapon wep, bool isEquipped)
        {
            if (weaponSlots.Count <= slotToSet)
            {
                SpawnNewSlot();
                SetWeaponSlot(slotToSet, wep, isEquipped);
            }
            else
            {
                if (weaponSlots[slotToSet].SetVisuals(wep, isEquipped))
                {
                    audioS.clip = swapWeaponSound;
                    audioS.Play();
                }
            }
        }

        public void SpawnNewSlot()
        {
            GameObject newSlot = Instantiate(newWeaponSlot, weaponSlotHolder);
            weaponSlots.Add(newSlot.GetComponent<UIWeaponSlot>());
        }

        public void SetAmmoVisuals(Weapon weapon)
        {
            RangedGun gun = weapon as RangedGun;
            if(gun != null)
            {
                SetAmmoVisualState(true);
                ammoImage.sprite = gun.ammoIcon;
                ammoText.text = gun.ammo.ToString();
            }
            else
            {
                SetAmmoVisualState(false);
            }
        }

        public void PlayerConsumableAnimation(Sprite newSprite)
        {
            afterAnimationSprite = newSprite;
            consumableAnim.SetTrigger("Swap");
            GameManager.OnAnimationEnd newDelegate = new GameManager.OnAnimationEnd(SetSpriteAfterAnimation);
            GameManager.instance.CheckWhenAnimationTagEnds(consumableAnim, "Swap", newDelegate);
        }

        public void SetConsumableVisual(List<Consumables.Consumable> consumables, int selected)
        {
            if (consumables.Count > 0)
            {
                if (consumables[selected].icon != null)
                {
                    consumableImage.gameObject.SetActive(true);
                    consumableImage.sprite = consumables[selected].icon;
                }
                else
                {
                    consumableImage.gameObject.SetActive(false);
                }
            }
            else
            {
                consumableImage.gameObject.SetActive(false);
            }
        }

        public void SetConsumableSprite(Sprite sprite)
        {
            consumableImage.sprite = sprite;
        }

        Sprite afterAnimationSprite;
        void SetSpriteAfterAnimation()
        {
            SetConsumableSprite(afterAnimationSprite);
        }

        public void SetAmmoVisualState(bool state)
        {
            ammoImage.gameObject.SetActive(state);
            ammoText.gameObject.SetActive(state);
        }

        public void AddStatusEffect(OngoingEffect effect)
        {
            print(effect.effectType.ToString());
            GameObject go = Instantiate(newStatusEffect, statusEffectsHolder);
            go.GetComponent<StatusEffectIcon>().SetupIcon(effect.effectData);
        }
    }
}
