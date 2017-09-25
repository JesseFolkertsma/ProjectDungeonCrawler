using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

namespace PDC.Consumables
{
    public class ConsumablePickup : MonoBehaviour
    {
        public Consumable consumable;

        private void OnTriggerEnter(Collider other)
        {
            PlayerCombat pc = other.transform.root.GetComponent<PlayerCombat>();
            if (pc != null)
            {
                pc.PickupConsumable(consumable);
                Destroy(gameObject);
            }
        }
    }
}
