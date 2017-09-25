using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PDC.Weapons;
using PDC.Consumables;


namespace PDC.NPCS
{
    [System.Serializable]
    public class NPCSentance
    {
        [TextArea]
        public string sentance;
        public NPCAction action;

        public Weapon weapon;
        public Consumable consumable;
        public string questName;
        public string questDescription;
    }

    public enum NPCAction
    {
        None,
        GiveWeapon,
        GiveConsumable,
        AddQuest,
    }
}
