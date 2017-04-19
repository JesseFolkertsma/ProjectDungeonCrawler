using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.StatusEffects;
using System;

public class Snake : BaseCharacter
{
    public StatusEffect effect;

    void Start()
    {
        effect.AddEffect(this);
    }
    public override void Attack()
    {
        throw new NotImplementedException();
    }

    public override void Die()
    {
        throw new NotImplementedException();
    }

    public override void Move()
    {
        throw new NotImplementedException();
    }
}
