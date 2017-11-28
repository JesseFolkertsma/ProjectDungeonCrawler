using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public WeaponData data;
    public Animator anim;
    public GameObject hitDecal;
    public GameObject weaponEffect;
    public Vector3 clientOffset;
    public Transform rightIK;
    public Transform leftIK;
    [HideInInspector] public NWPlayerCombat pc;

    [HideInInspector]
    public float timer = 0;

    public void Setup(NWPlayerCombat _pc, bool isLocal)
    {
        pc = _pc;
        if (!isLocal)
        {
            transform.localPosition = clientOffset;
        }
    }

    public virtual void Attack()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
    }

    public virtual void AttackButtonUp()
    {
        anim.SetBool("Attacking", false);
    }

    public virtual void PlayVisuals() { }

    public virtual void WeaponEffects(Vector3 hitpos, Quaternion hitrot)
    {
        if (hitDecal != null)
        {
            Instantiate(hitDecal, hitpos, hitrot);
        }
    }

    public bool IsInBaseState()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsTag("Base"));
    }
}
