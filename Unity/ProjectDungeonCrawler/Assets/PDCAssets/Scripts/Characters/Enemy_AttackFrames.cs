using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;

public class Enemy_AttackFrames : MonoBehaviour {

    private Enemy myEnemy;

    private void Awake()
    {
        myEnemy = transform.root.GetComponent<Enemy>();
    }

    public void SwitchDamageFrames()
    {
        myEnemy.SwitchDamageFrames();
    }

    public void EndAttack()
    {
        myEnemy.EndAttack();
    }
}
