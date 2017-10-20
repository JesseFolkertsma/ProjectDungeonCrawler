using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponVisuals : MonoBehaviour
{
    public Animator anim;
    [HideInInspector] public NWPlayerCombat pc;

    public void Setup(NWPlayerCombat _pc)
    {
        pc = _pc;
    }

    public virtual void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public bool IsInBaseState()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsTag("Base"));
    }

    public abstract void RemotePlayerStartVisuals();
    public abstract void RemotePlayerEffectVisuals();
}
