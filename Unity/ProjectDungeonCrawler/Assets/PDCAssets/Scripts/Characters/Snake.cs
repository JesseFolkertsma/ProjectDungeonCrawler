using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.StatusEffects;
using System;

namespace PDC.Characters
{
    public class Snake : AICharacter
    {
        public StatusEffect effect;
        bool attacking = false;

        void Awake()
        {
            //effect.AddEffect(this);
            SetupAI();
        }

        void Update()
        {
            if (!attacking)
                UpdateAI();
        }

        public override void Attack()
        {
            print("ATTACKZZZ");
            anim.SetTrigger("Attack");
        }

        public void StartAttack()
        {
            attacking = true;
        }

        public void JumpAttack()
        {
            rb.velocity += (player.transform.position - transform.position).normalized * 10000;
        }

        public void StopAttack()
        {
            attacking = false;
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
}
