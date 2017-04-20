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
        public GameObject snekdoll;

        void Start()
        {
            SetupAI();
        }
        void Update()
        {
            AIUpdate();
        }
        public override void Die()
        {
            if (!isdead)
            {
                Instantiate(snekdoll, transform.position, transform.rotation);
                Destroy(gameObject);
                isdead = true;
            }
        }
    }
}
