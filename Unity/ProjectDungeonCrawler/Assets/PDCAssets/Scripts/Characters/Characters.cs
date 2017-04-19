using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Abilities;
using PDC.StatusEffects;

namespace PDC
{
    namespace Characters
    {
        [System.Serializable]
        public abstract class BaseCharacter : MonoBehaviour
        {
            public string characterName = "New Character";
            public Stats characterStats;
            public List<OngoingEffect> ongoingEffects = new List<OngoingEffect>();

            public void TakeDamage(float damage)
            {
                characterStats.currentHP -= damage;
                if(characterStats.currentHP <= 0)
                    Die();
            }

            public void GiveStatusEffect(OngoingEffect effect)
            {
                effect.routine = StartCoroutine(effect.effect);
                ongoingEffects.Add(effect);
            }

            public void CureEffects()
            {
                foreach(OngoingEffect oge in ongoingEffects)
                {
                    StopCoroutine(oge.effect);
                }
            }

            public abstract void Move();

            public abstract void Attack();

            public abstract void Die();
        }

        [System.Serializable]
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
