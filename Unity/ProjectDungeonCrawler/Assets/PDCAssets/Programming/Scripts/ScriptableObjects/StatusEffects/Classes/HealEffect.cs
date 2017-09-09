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

        public override void AddEffect(BaseCharacter character)
        {
            if (instant)
            {
                character.Heal(healPerTick);
            }
            else
            {
                character.GiveStatusEffect(new OngoingEffect(type, Heal(character, Time.time), this));
            }
        }

        IEnumerator Heal(BaseCharacter character, float startTime)
        {
            while (startTime + effectDuration > Time.time)
            {
                character.Heal(healPerTick);
                yield return new WaitForSeconds(1 / tickRate);
            }
        }
    }
}
