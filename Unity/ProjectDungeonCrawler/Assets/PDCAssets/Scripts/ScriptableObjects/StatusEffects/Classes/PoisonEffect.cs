using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

namespace PDC.StatusEffects
{
    [CreateAssetMenu(menuName = "PDC/StatusEffects/PoisonEffect")]
    public class PoisonEffect : StatusEffect
    {
        public float tickRate = 1;
        public float damagePerTick = 3;
        public float effectDuration = 5;

        public override void AddEffect(BaseCharacter character)
        {
            character.GiveStatusEffect(new OngoingEffect(type, Poison(character, Time.time)));
        }

        IEnumerator Poison(BaseCharacter character, float startTime)
        {
            while (startTime + effectDuration > Time.time)
            {
                character.TakeDamage(damagePerTick, type);
                yield return new WaitForSeconds(1 / tickRate);
            }
        }
    }
}
