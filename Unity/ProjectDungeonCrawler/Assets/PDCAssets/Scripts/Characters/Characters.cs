using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC
{
    namespace Characters
    {
        public abstract class BaseCharacter : MonoBehaviour
        {
            public string characterName = "New Character";
            public Stats characterStats;
            List<Coroutine> statusEffects;

            public void TakeDamage(float damage)
            {
                characterStats.currentHP -= damage;
                if(characterStats.currentHP <= 0)
                    Die();
            }

            public void GiveStatusEffect(Coroutine effect)
            {
                statusEffects.Add(effect);
            }

            public void CureEffects()
            {
                foreach(Coroutine c in statusEffects)
                {
                    StopCoroutine(c);
                }
            }

            public abstract void Move();

            public abstract void Attack();

            public abstract void Die();
        }

        [SerializeField]
        public class Stats
        {
            [SerializeField]
            float maxHP = 100;
            public float currentHP;
            [SerializeField]
            float maxSouls = 100;
            public float currentSouls;
            public float armorRating;
            public float movementSpeed;

            public float MaxHP
            {
                get
                {
                    return maxHP;
                }
            }

            public float MaxSouls
            {
                get
                {
                    return maxSouls;
                }
            }
        }
    }
}
