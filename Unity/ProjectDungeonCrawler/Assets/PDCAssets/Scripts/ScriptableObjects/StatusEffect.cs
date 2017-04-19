using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;

namespace PDC.StatusEffects
{
    public enum EffectType
    {
        Buff = 0,
        Poison = 1,
        Heal = 2,
    };

    public class OngoingEffect
    {
        public EffectType effectType;
        public IEnumerator effect;
        public Coroutine routine;

        public OngoingEffect(EffectType type, IEnumerator eff)
        {
            effectType = type;
            effect = eff;
        }
    }

    public abstract class StatusEffect : ScriptableObject
    {
        public string effectName = "New Status Effect";
        public EffectType type = EffectType.Buff;
        public abstract void AddEffect(BaseCharacter character);
    }
}


