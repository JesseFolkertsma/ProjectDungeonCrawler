using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;
using System;
using PDC.Characters;

public class Spear : BaseWeapon
{
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
        hc.anim.SetTrigger("LightAttack");
    }

    public override void HeavyAttack(HumanoidCharacter hc)
    {
        hc.anim.SetTrigger("HeavyAttack");
    }
}
