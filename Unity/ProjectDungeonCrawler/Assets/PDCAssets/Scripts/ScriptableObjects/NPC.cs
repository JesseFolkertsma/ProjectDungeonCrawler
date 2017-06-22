using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        public string sentance;
        public UnityAction action;
    }
}
