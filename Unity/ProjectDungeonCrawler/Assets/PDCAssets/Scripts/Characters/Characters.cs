using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Abilities;
using PDC.StatusEffects;
using System;
using UnityEngine.AI;

namespace PDC
{
    namespace Characters
    {
        [System.Serializable]
        public abstract class BaseCharacter : MonoBehaviour
        {
            public string characterName = "New Character";
            public Stats characterStats;
            public List<OngoingEffect> ongoingEffects = new List<OngoingEffect>();

            public void TakeDamage(float damage)
            {
                characterStats.currentHP -= damage;
                if(characterStats.currentHP <= 0)
                    Die();
            }

            public void GiveStatusEffect(OngoingEffect effect)
            {
                effect.routine = StartCoroutine(effect.effect);
                ongoingEffects.Add(effect);
            }

            public void CureEffects()
            {
                foreach(OngoingEffect oge in ongoingEffects)
                {
                    StopCoroutine(oge.effect);
                }
            }

            public abstract void Move();

            public abstract void Attack();

            public abstract void Die();
        }

        public enum AIState
        {
            Idle,
            Searching,
            Persueing,
        }

        public class AICharacter : BaseCharacter
        {
            public AIState state = AIState.Idle;
            public Rigidbody rb;
            public Animator anim;
            public float sightRange;
            public float attackRange = 4;
            public GameObject player;
            [SerializeField]
            Transform headBone;
            NavMeshAgent agent;
            [SerializeField]
            GameObject hitbox;

            public void SetupAI()
            {
                rb = GetComponent<Rigidbody>();
                anim = GetComponent<Animator>();
                player = GameObject.FindGameObjectWithTag("Player");
                agent = GetComponent<NavMeshAgent>();
                agent.isStopped = true;
                agent.stoppingDistance = attackRange - .5f;
            }

            float startSearch = 0f;
            public void UpdateAI()
            {
                switch (state)
                {
                    case AIState.Idle:
                        if (CheckForPlayer())
                        {
                            state = AIState.Persueing;
                            agent.isStopped = false;
                        }
                        break;
                    case AIState.Searching:
                        if (CheckForPlayer())
                        {
                            state = AIState.Persueing;
                            agent.isStopped = false;
                        }
                        break;
                    case AIState.Persueing:
                        agent.destination = player.transform.position;
                        if (PlayerDistance() > 20)
                        {
                            state = AIState.Searching;
                        }
                        else if(agent.remainingDistance < attackRange)
                        {
                            Attack();
                        }
                        break;
                }
            }

            bool CheckForPlayer()
            {
                return true;
                Vector3 dir = (headBone.position - player.transform.position).normalized;
                RaycastHit hit;
                if(Physics.Raycast(headBone.position, dir, out hit, sightRange))
                {
                    if (hit.transform.tag == "Player")
                    {
                        return true;
                    }
                }
                return false;
            }

            float PlayerDistance()
            {
                return Vector3.Distance(transform.position, player.transform.position);
            }

            public void ActivateHitbox()
            {
                hitbox.SetActive(true);
            }

            public void DeactivateHitbox()
            {
                hitbox.SetActive(false);
            }

            public override void Attack()
            {
                throw new NotImplementedException();
            }

            public override void Die()
            {
                throw new NotImplementedException();
            }

            public override void Move()
            {
                throw new NotImplementedException();
            }
        }

        [System.Serializable]
        public class Stats
        {
            [SerializeField]
            float maxHP = 100;
            public float currentHP;
            [SerializeField]
            float maxSouls = 100;
            public float currentSouls;
            public float armorRating;
            public float movementSpeed;

            public float MaxHP
            {
                get
                {
                    return maxHP;
                }
            }

            public float MaxSouls
            {
                get
                {
                    return maxSouls;
                }
            }
        }
    }
}
