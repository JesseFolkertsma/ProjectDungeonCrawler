using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;

namespace PDC
{
    public enum EffectType
    {
        Normal = 0,
        Buff = 1,
        Poison = 2,
        Heal = 3,
    };
}

namespace PDC.StatusEffects
{
    public class OngoingEffect
    {
        public EffectType effectType;
        public IEnumerator effect;
        public Coroutine routine;
        public StatusEffect effectData;

        public OngoingEffect(EffectType type, IEnumerator eff, StatusEffect eData)
        {
            effectType = type;
            effect = eff;
            effectData = eData;
        }
    }

    public abstract class StatusEffect : ScriptableObject
    {
        public EffectType type = EffectType.Buff;
        public abstract void AddEffect(BaseCharacter character);
        public Sprite effectIcon;
        public Color effectColor;
        public float effectDuration = 5;
    }
}


