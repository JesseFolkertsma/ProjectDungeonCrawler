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
}
