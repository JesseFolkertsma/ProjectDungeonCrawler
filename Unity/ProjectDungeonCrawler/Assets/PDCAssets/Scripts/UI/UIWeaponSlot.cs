using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDC.Weapons;
using PDC.StatusEffects;

namespace PDC.UI
{
    public class UIWeaponSlot : MonoBehaviour {

        public Image weaponImage;
        public Image[] statusEffects;
        public RectTransform slot;

        Coroutine routine;

        public void SetVisuals(Weapon weapon, bool equipped)
        {
            if (equipped)
            {
                //slot.localPosition = new Vector2(25, 0);
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
                routine = StartCoroutine(SelectSlot(150));
            }
            else
            {
                //slot.localPosition = new Vector2(0, 0);
                if(routine != null)
                {
                    StopCoroutine(routine);
                }
                routine = StartCoroutine(DeSelectSlot(150));
            }
            if (weapon == null)
            {
                weaponImage.gameObject.SetActive(false);
                foreach (Image i in statusEffects)
                {
                    i.gameObject.SetActive(false);
                }
            }
            else
            {
                weaponImage.gameObject.SetActive(true);
                weaponImage.sprite = weapon.weaponIcon;
                for (int i = 0; i < weapon.weaponEffects.Length; i++)
                {
                    if (i > statusEffects.Length)
                        break;

                    statusEffects[i].gameObject.SetActive(true);
                    statusEffects[i].color = weapon.weaponEffects[i].effectColor;
                }
            }
        }

        IEnumerator SelectSlot(float speed)
        {
            while(slot.localPosition.x < 24.5)
            {
                slot.localPosition = Vector3.MoveTowards(slot.localPosition, new Vector2(25, 0), Time.deltaTime * speed);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator DeSelectSlot(float speed)
        {
            while (slot.localPosition.x > .05f)
            {
                slot.localPosition = Vector3.MoveTowards(slot.localPosition, new Vector2(0, 0), Time.deltaTime * speed);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
