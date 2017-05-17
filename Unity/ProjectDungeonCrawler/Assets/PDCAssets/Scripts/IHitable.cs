using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC
{
    public interface IHitable
    {
        void GetHit(float damage, EffectType hitType);
    }
}
