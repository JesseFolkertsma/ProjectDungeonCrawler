using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDC.Weapons;

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

        [Header("Standalone Objects")]
        [SerializeField] GameObject newQuestObject;
        [SerializeField] GameObject newWeaponSlot;

        public void SetHp(float currentHP, float maxHp)
        {
            if(currentHP < 0)
                hpImage.fillAmount = 0;
            else
                hpImage.fillAmount = currentHP / maxHp;
        }

        public void SetCaveProgression(float percentage)
        {
            caveProgressionImage.fillAmount = percentage;
        }

        public void AddQuest(Quest questToAdd)
        {
            GameObject newQuest = Instantiate(newQuestObject, questLog);
            Text questText = newQuest.GetComponent<Text>();
            questText.text = questToAdd.inGameDesc;
            quests.Add(questText);
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
                weaponSlots[slotToSet].SetVisuals(wep, isEquipped);
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

        public void SetConsumableVisual(Sprite sprite)
        {
            if (sprite != null)
            {
                consumableImage.gameObject.SetActive(true);
                consumableImage.sprite = sprite;
            }
            else
            {
                consumableImage.gameObject.SetActive(false);
            }
        }

        Sprite afterAnimationSprite;
        void SetSpriteAfterAnimation()
        {
            SetConsumableVisual(afterAnimationSprite);
        }

        public void SetAmmoVisualState(bool state)
        {
            ammoImage.gameObject.SetActive(state);
            ammoText.gameObject.SetActive(state);
        }
    }
}
