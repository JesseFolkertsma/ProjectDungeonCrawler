using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.StatusEffects;
using System;

public class Snake : AICharacter
{
    public StatusEffect effect;

    void Awake()
    {
        //effect.AddEffect(this);
        SetupAI();
    }

    void Update()
    {
        UpdateAI();
    }

    public override void Attack()
    {
        print("ATTACKZZZ");
        //rb.velocity += (transform.position - player.transform.position).normalized * 10000;
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
