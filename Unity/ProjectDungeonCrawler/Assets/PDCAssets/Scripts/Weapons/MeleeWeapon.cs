using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.StatusEffects;

public class MeleeWeapon : MonoBehaviour {

    public float damage = 10f;
    public StatusEffect[] effects;
    bool canhit = false;

    void OnTriggerEnter(Collider col)
    {
        BaseCharacter hit = col.transform.root.GetComponentInChildren<BaseCharacter>();
        hit.TakeDamage(damage);
        foreach(StatusEffect eff in effects)
        {
            eff.AddEffect(hit);
        }
    }

    void OnEnable()
    {
        canhit = true;
    }

    void OnDisable()
    {
        canhit = false;
    }
}
