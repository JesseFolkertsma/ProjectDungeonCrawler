using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC;
using PDC.StatusEffects;
using PDC.Characters;
using System;

namespace PDC.Weapons
{
    public enum WeaponType
    {
        Revolver,
        Spear,
        Shotgun,
        Bow,
        Gatling,
        Axe,
    }

    [System.Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    public abstract class Weapon : MonoBehaviour
    {
        [Header("Weapon game data")]
        public string weaponName = "New Weapon";
        public WeaponType type;
        public Sprite weaponIcon;

        [Header("Weapon stats")]
        public float damage = 20;
        public float throwDamage = 20;
        public StatusEffect[] weaponEffects;
        public GameObject physicsCol;

        [Header("Tweaks")]
        public Vector3 weaponHolderPositionOffset;
        public Vector3 weaponHolderRotationOffset;

        [HideInInspector] public bool isEquipped;
        [HideInInspector] public int assignedSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        [HideInInspector] public AudioSource audioS;
        [HideInInspector] public bool canAttack = true;

        [HideInInspector] public PlayerCombat pc;

        public abstract void Fire1Hold(Camera playercam, LayerMask mask);
        public abstract void Fire1Up();
        public abstract void Fire2Hold();
        public abstract void Fire2Up();
        public abstract void Attack();

        private void Awake()
        {
            OnStart();
        }

        public virtual void OnStart()
        {
            canAttack = true;
            rb = GetComponent<Rigidbody>();
            if (GetComponent<Animator>() != null)
                anim = GetComponent<Animator>();
            audioS = GetComponent<AudioSource>();
        }

        public void Throw()
        {
            pc.ThrowWeapon();
        }

        public virtual void ThrowWeapon(Camera playercam, float strenght)
        {
            Invoke("UnEquip", .05f);
            SetLayerRecursively(gameObject, "Weapon");
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
            transform.parent = null;
            rb.isKinematic = false;
            physicsCol.SetActive(true);
            rb.AddForce((playercam.transform.forward * strenght) + playercam.transform.up * (strenght / 6));
        }

        void UnEquip()
        {
            isEquipped = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!isEquipped)
            {
                if (collision.transform.root.tag == "Player")
                {
                    collision.transform.root.GetComponent<PlayerCombat>().PickupWeapon(this);
                    print("Getting pickedup boiii");
                }
            }
        }

        public virtual void OnPickup()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isEquipped)
            {
                if (collision.transform.root.tag != "Player")
                    if (rb.velocity.magnitude > 1)
                    {
                        IHitable iHit = collision.transform.GetComponent<IHitable>();
                        if (iHit != null)
                            ThrowHitEnemy(iHit, collision.transform.position);
                    }
            }
        }

        public virtual void ThrowHitEnemy(IHitable iHit, Vector3 hitPos)
        {
            iHit.GetHit(damage, EffectType.Normal, weaponEffects, hitPos);
            rb.velocity = Vector3.zero;
            Vector3 playerdir = ((PlayerCombat.instance.transform.position - transform.position) * 100) + (Vector3.up * 200);
            rb.AddForce(playerdir);
        }

        public void SetLayerRecursively(GameObject go, string layerName)
        {
            if (go == null) return;

            if(go.layer != LayerMask.NameToLayer("Hitbox"))
                go.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in go.transform)
            {
                if (child == null) return;
                SetLayerRecursively(child.gameObject, layerName);
            }
        }
    }

    [System.Serializable]
    public abstract class RangedGun : Weapon
    {
        [Header("Ranged gun stats")]
        public float range = 100;
        [Tooltip("Location of muzzleflash")]
        public Transform gunEnd;
        public GameObject muzzleFlash;
        public AudioClip dryfireSound;
        public Sprite ammoIcon;
        public int ammoUsePerShot = 1;
        public int maxAmmo = 8;
        public int ammo = 8;
        public Vector2 minMaxAmmoReturnOnBounce;

        public Transform bulletEjectPosition;
        public GameObject bulletEjectEffect;
        public float ejectForce = 100;

        [HideInInspector] public Camera cam;
        [HideInInspector] public LayerMask m;
        
        public void DamageRaycast()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, m))
            {
                IHitable iHit = hit.transform.GetComponent<IHitable>();
                if (iHit != null)
                    iHit.GetHit(damage, EffectType.Normal, weaponEffects, cam.transform.position);
            }
        }

        public virtual void DryFire()
        {
            anim.SetTrigger("DryFire");
        }

        public void PlaySound(AudioClip sound)
        {
            audioS.clip = sound;
            audioS.Play();
        }

        public virtual void ShootVisuals()
        {
            if(muzzleFlash != null)
                Instantiate(muzzleFlash, gunEnd.position, gunEnd.rotation);

            if(bulletEjectEffect != null)
            {
                GameObject go = Instantiate(bulletEjectEffect, bulletEjectPosition.position, Quaternion.LookRotation(bulletEjectPosition.right));
                go.GetComponent<Rigidbody>().AddForce(bulletEjectPosition.forward * ejectForce);
            }
        }

        public override void ThrowHitEnemy(IHitable iHit, Vector3 hitPos)
        {
            base.ThrowHitEnemy(iHit, hitPos);
            int ammoReturn = (int)UnityEngine.Random.Range(minMaxAmmoReturnOnBounce.x, minMaxAmmoReturnOnBounce.y);
            ammo += ammoReturn;
            if (ammo > maxAmmo)
                ammo = maxAmmo;
        }
    }
}
