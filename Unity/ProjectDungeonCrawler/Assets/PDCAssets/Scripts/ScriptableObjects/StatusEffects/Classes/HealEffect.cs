using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

namespace PDC.StatusEffects
{
    [CreateAssetMenu(menuName = "PDC/StatusEffects/HealEffect")]
    public class HealEffect : StatusEffect
    {
        public bool instant = false;
        public float healPerTick = 3;
        [Header("Only use varables below if 'Instant' is unchecked")]
        public float tickRate = 1;
        public float effectDuration = 5;

        public override void AddEffect(BaseCharacter character)
        {
            if (instant)
            {
                character.characterStats.currentHP += healPerTick;
            }
            else
            {
                character.GiveStatusEffect(new OngoingEffect(type, Heal(character, Time.time)));
            }
        }

        IEnumerator Heal(BaseCharacter character, float startTime)
        {
            while (startTime + effectDuration > Time.time)
            {
                character.characterStats.currentHP += healPerTick;
                yield return new WaitForSeconds(1 / tickRate);
            }
        }
    }
}
