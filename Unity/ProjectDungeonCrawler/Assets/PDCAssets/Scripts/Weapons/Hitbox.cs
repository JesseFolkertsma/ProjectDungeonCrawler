using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

namespace PDC.Weapons
{
    public class Hitbox : MonoBehaviour
    {
        MeleeWeapon attachedWeapon;

        bool isEnabled = false;
        bool isSetup = false;

        public void Enable()
        {
            isEnabled = true;
        }

        public void Disable()
        {
            isEnabled = false;
        }

        public void SetupHitbox(MeleeWeapon wep)
        {
            attachedWeapon = wep;
            isSetup = true;
        }

        public void ClearHits()
        {
            hits = new List<IHitable>();
        }

        List<IHitable> hits = new List<IHitable>();
        private void OnTriggerEnter(Collider other)
        {
            if (isEnabled && attachedWeapon != null)
            {
                IHitable iHit = other.transform.GetComponent<IHitable>();
                if (iHit != null)
                {
                    if (!hits.Contains(iHit))
                    {
                        attachedWeapon.HitboxHit(iHit);
                        hits.Add(iHit);
                    }
                }
            }
        }
    }
}
