using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

public abstract class Pickup : MonoBehaviour {
    public GameObject pickupCol;
    public GameObject lightEffect;
    public Rigidbody rb;
    public bool canPickup = true;
    public bool isThrown = false;

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.root.GetComponentInChildren<BaseCharacter>())
        {
            if (col.transform.root.GetComponentInChildren<HumanoidCharacter>())
            {
                if (canPickup)
                {
                    PickupItem(col.transform.root.GetComponentInChildren<HumanoidCharacter>());
                }
            }
            if (isThrown)
            {
                col.transform.root.GetComponentInChildren<BaseCharacter>().TakeDamage(50, Color.red);
            }
        }
    }

    public void PickUpdate()
    {
        if (isThrown)
        {
            if (rb.velocity.magnitude < 2)
            {
                isThrown = false;
                canPickup = true;
                lightEffect.SetActive(true);
            }
        }
    }

    public abstract void PickupItem(HumanoidCharacter hc);
}
