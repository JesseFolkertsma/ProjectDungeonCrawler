using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.StatusEffects;

namespace PDC.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public void UpdateWeaponVisual(List<Weapon> weapons, Weapon equipped)
        {
            if(equipped != null)
                UpdateAmmo(equipped.weaponIcon, equipped.ammo);
        }

        public void UpdateAmmo(Sprite ammoSprite, int ammo)
        {

        }


    }
}
