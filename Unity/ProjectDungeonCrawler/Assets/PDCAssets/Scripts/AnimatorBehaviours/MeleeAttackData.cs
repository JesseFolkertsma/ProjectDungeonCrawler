using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;

public enum AttackType
{
    Horizontal,
    Vertical
}

public class MeleeAttackData : StateMachineBehaviour
{
    public AttackType attackType;
    public float hitAngle;
    public float attackWidthOrHeight;
    public Vector3 angleDirectionOffset;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<MeleeWeapon>().SetupAttackData(this);
	}
}
