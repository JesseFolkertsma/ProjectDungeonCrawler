﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.StatusEffects;

namespace PDC.Consumables
{
    public abstract class Consumable : ScriptableObject
    {
        public Sprite icon;
        public abstract bool Use(PlayerCombat pc);
    }
}
