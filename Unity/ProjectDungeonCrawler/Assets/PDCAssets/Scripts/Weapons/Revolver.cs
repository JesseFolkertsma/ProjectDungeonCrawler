using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using PDC.Characters;
using PDC.StatusEffects;
using System;

public class Revolver : BaseWeapon
{
    public Transform gunEnd;
    public GameObject flash;
    public int ammo = 8;

    void Start()
    {
        SetupWeapon();
    }
    
    void Update()
    {
        PickUpdate();
    }

    public override void LightAttack(HumanoidCharacter hc)
    {
        if (ammo > 0)
        {
            ammo--;
            Instantiate(flash, gunEnd.position, Quaternion.identity);
            hc.anim.SetTrigger("LightAttack");
            RaycastHit hit;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 50);
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100))
            {
                BaseCharacter hitchar = hit.transform.root.GetComponent<BaseCharacter>();
                if (hitchar != null)
                {
                    hitchar.TakeDamage(damage);
                    foreach (StatusEffect se in effects)
                    {
                        se.AddEffect(hitchar);
                    }
                }
            }
        }
    }

    public override void HeavyAttack(HumanoidCharacter hc)
    {
        hc.anim.SetTrigger("HeavyAttack");
    }
}
