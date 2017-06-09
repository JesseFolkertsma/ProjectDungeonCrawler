using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.StatusEffects;
using PDC.Characters;

namespace PDC.Consumables
{
    [CreateAssetMenu(menuName = "PDC/Consumables/ConsumablePotion")]
    public class ConsumablePotion : Consumable
    {
        public StatusEffect effect;

        public override bool Use(PlayerCombat pc)
        {
            effect.AddEffect(pc);
            return true;
        }
    }
}
