using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.StatusEffects;

namespace PDC
{
    public interface IHitable
    {
        void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition);
    }
}
