using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using PDC.Characters;

namespace PDC.Weapons
{
    public enum WeaponType
    {
        Revolver,
        Spear,
        Shotgun,
        Bow,
    }

    [System.Serializable]
    public abstract class Weapon : MonoBehaviour
    {
        public string weaponName = "New Weapon";
        public WeaponType type;
        public Sprite weaponIcon;
        public Sprite ammoIcon;
        public float damage = 20;
        public float throwDamage = 20;
        public StatusEffect[] weaponEffects;
        public int maxAmmo = 8;
        public int ammo = 8;
        public bool isEquipped;

        [HideInInspector] public int assignedSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        public GameObject physicsCol;
        [HideInInspector] public bool canAttack = true;

        public abstract void Fire1Hold(Camera playercam, LayerMask mask);
        public abstract void Fire1Up();
        public abstract void Fire2Hold();
        public abstract void Fire2Up();
        public abstract void Attack();

        private void Awake()
        {
            canAttack = true;
            rb = GetComponent<Rigidbody>();
            if(GetComponent<Animator>() != null)
                anim = GetComponent<Animator>();
        }

        public void Throw(Camera playercam, float strenght)
        {
            SetLayerRecursively(gameObject, "Default");
            transform.position = transform.parent.position;
            transform.parent = null;
            rb.isKinematic = false;
            physicsCol.SetActive(true);
            rb.AddForce((playercam.transform.forward * strenght) + playercam.transform.up * (strenght / 6));
            Invoke("UnEquip", .05f);
        }

        void UnEquip()
        {
            isEquipped = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isEquipped)
            {
                if (collision.transform.root.tag == "Player")
                {
                    collision.transform.root.GetComponent<PlayerCombat>().PickupWeapon(this);
                    print("Getting pickedup boiii");
                }
                else if (rb.velocity.magnitude > 1)
                {
                    IHitable iHit = collision.transform.GetComponent<IHitable>();
                    if (iHit != null)
                    {
                        iHit.GetHit(damage, EffectType.Normal, weaponEffects, collision.transform.position);
                        rb.velocity = Vector3.zero;
                        Vector3 playerdir = ((PlayerController.instance.transform.position - transform.position) * 100) + (Vector3.up * 200);
                        rb.AddForce(playerdir);
                    }
                }
            }
        }

        public void SetLayerRecursively(GameObject go, string layerName)
        {
            if (go == null) return;

            go.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in go.transform)
            {
                if (child == null) return;
                SetLayerRecursively(child.gameObject, layerName);
            }
        }

        public delegate void OnAnimationEnd();
        public void CheckWhenAnimationEnds(Animator anim, string animationName, OnAnimationEnd effectAfterEnd)
        {
            StartCoroutine(AnimatorCheckRoutine(anim, animationName, effectAfterEnd));
        }

        IEnumerator AnimatorCheckRoutine(Animator anim, string animationName, OnAnimationEnd effectAfterEnd)
        {
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return new WaitForEndOfFrame();
            }
            while (anim.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                yield return new WaitForEndOfFrame();
            }
            print("Animation: " + animationName + " has ended.");
            effectAfterEnd();
        }
    }
}
