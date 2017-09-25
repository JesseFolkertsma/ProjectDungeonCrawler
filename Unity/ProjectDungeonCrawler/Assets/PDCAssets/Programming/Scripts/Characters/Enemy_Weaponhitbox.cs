using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.Characters;

public class Enemy_Weaponhitbox : MonoBehaviour {

    private Enemy myEnemy;

    private void Awake()
    {
        myEnemy = transform.root.GetComponent<Enemy>();
    }

	private void OnTriggerEnter(Collider c)
    {
        IHitable hit = (IHitable)c.transform.root.GetComponentInChildren(typeof(IHitable));
        if(hit != null)
            myEnemy.HitObject(hit);
    }
}
