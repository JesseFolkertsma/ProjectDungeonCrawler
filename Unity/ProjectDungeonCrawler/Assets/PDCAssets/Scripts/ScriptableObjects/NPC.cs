using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PDC.Weapons;
using PDC.Consumables;

namespace PDC.NPCS
{
    [CreateAssetMenu(menuName = "PDC/NPC's/New NPC")]
    public class NPC : ScriptableObject
    {
        public Sprite npcSprite;

        public NPCSentance[] conversation;
    }

    [System.Serializable]
    public class NPCSentance
    {
        [TextArea]
        public string sentance;
        public NPCAction action;

        public Weapon weapon;
        public Consumable consumable;
        public Quest quest;
    }

    public enum NPCAction
    {
        None,
        GiveWeapon,
        GiveConsumable,
        AddQuest,
    }
}
