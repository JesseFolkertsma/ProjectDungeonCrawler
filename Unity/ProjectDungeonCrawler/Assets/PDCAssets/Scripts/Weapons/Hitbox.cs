using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.Characters;
using PDC.StatusEffects;

[System.Serializable]
public class Hitbox : MonoBehaviour {

    bool canHit = false;
    BaseWeapon attachedWeapon;

    void Start()
    {
        canHit = false;
        attachedWeapon = transform.parent.GetComponent<BaseWeapon>();
    }

	public void Enable()
    {
        gameObject.SetActive(true);
        canHit = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        canHit = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (canHit)
        {
            BaseCharacter bc = col.transform.root.GetComponentInChildren<BaseCharacter>();
            bc.TakeDamage(attachedWeapon.damage);
            foreach (StatusEffect se in attachedWeapon.effects)
            {
                se.AddEffect(bc);
            }
            canHit = false;
        }
    }
}
