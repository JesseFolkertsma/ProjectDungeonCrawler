using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public WeaponData data;
    public Animator anim;
    public GameObject hitDecal;
    public GameObject weaponEffect;
    [HideInInspector] public NWPlayerCombat pc;

    public void Setup(NWPlayerCombat _pc)
    {
        pc = _pc;
    }

    public virtual bool Attack()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("Attacking", true);
        return true;
    }

    public virtual void AttackButtonUp()
    {
        anim.SetBool("Attacking", false);
    }

    public virtual void PlayVisuals() { }

    public virtual void WeaponEffects(Vector3 hitpos, Quaternion hitrot)
    {
        if(hitDecal != null)
            Instantiate(hitDecal, hitpos, hitrot);
    }

    public bool IsInBaseState()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsTag("Base"));
    }
}
