using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.NPCS
{
    [CreateAssetMenu(menuName = "PDC/NPC's/New NPC")]
    public class NPC : ScriptableObject
    {
        public Sprite npcSprite;

        public string[] conversation;
    }
}
