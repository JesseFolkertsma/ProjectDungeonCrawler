using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Abilities;
using PDC.StatusEffects;
using System;
using UnityEngine.AI;

namespace PDC.Characters
{
    [Serializable]
    public abstract class BaseCharacter : MonoBehaviour
    {
        public string characterName = "New Character";
        public Stats characterStats;
        public List<OngoingEffect> ongoingEffects = new List<OngoingEffect>();
        public bool isdead = false;

        public virtual void TakeDamage(float damage, EffectType damageType)
        {
            characterStats.currentHP -= damage;
            if(characterStats.currentHP <= 0)
                Die();
        }

        public virtual void Heal(float hp)
        {
            if (characterStats.currentHP >= characterStats.MaxHP)
                return;

            characterStats.currentHP += hp;
            if(characterStats.currentHP >= characterStats.MaxHP)
                characterStats.currentHP = characterStats.MaxHP;
        }

        public virtual void GiveStatusEffect(OngoingEffect effect)
        {
            effect.routine = StartCoroutine(effect.effect);
            ongoingEffects.Add(effect);
        }

        public virtual void CureEffects()
        {
            foreach (OngoingEffect oge in ongoingEffects)
            {
                StopCoroutine(oge.effect);
            }
        }

        public virtual void CureEffectOfType(EffectType type)
        {
            foreach (OngoingEffect oge in ongoingEffects)
            {
                if (oge.effectType == type)
                {
                    StopCoroutine(oge.effect);
                }
            }
        }

        public virtual void CureSpecificEffect(OngoingEffect effectToCure)
        {
            foreach (OngoingEffect oge in ongoingEffects)
            {
                if (oge == effectToCure)
                {
                    StopCoroutine(oge.effect);
                    break;
                }
            }
        }

        public abstract void Die();
    }

    public enum AIState
    {
        Idle,
        LateIdle,
        InRange,
        InView,
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
