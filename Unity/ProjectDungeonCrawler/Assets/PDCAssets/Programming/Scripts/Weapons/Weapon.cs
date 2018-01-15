using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public WeaponData data;
    public Animator anim;
    public Animator overallAnim;
    public GameObject hitDecal;
    public GameObject weaponEffect;
    public GameObject mesh;
    public Vector3 clientOffset;
    public Transform rightIK;
    public Transform leftIK;
    public bool canReload = true;
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
        overallAnim = transform.parent.parent.GetComponent<Animator>();
    }

    public virtual void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public virtual void RightClick(bool down) { }

    public virtual void PlayVisuals() { }

    public virtual void WeaponEffects(Vector3 hitpos, Quaternion hitrot)
    {
        if (hitDecal != null)
        {
            GameManager.instance.SpawnObject("Decal", hitpos, hitrot);
        }
    }

    public bool IsInBaseState()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsTag("Base") && overallAnim.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
    }
}
