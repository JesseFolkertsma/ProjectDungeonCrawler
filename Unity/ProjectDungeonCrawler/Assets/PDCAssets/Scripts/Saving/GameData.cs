using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using PDC.Weapons;
using PDC.StatusEffects;
using PDC.Consumables;

namespace PDC.Saving
{
    [System.Serializable]
    public class GameData
    {
        public Stats playerStats = new Stats();
        public List<int> playerWeapons = new List<int>();
        public List<Consumable> playerConsumables = new List<Consumable>();
    }
}
