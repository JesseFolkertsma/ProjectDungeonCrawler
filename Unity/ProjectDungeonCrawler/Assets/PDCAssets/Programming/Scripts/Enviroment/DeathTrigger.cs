using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public enum DeathType
    {
        Water,
    };

    public DeathType deathType;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<NWPlayerCombat>().EnviromentDeath(deathType);
    }
}
