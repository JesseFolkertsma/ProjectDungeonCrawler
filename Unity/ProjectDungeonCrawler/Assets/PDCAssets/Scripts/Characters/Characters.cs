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
            LateIdle,
            InRange,
            InView,
        }

        public class AICharacter : BaseCharacter
        {
            [Header("AI Controller variables")]
            public Transform playerTarget;
            public Transform headBone;
            public AIState state = AIState.Idle;

            //Public variables
            public float lookRange = 15;
            public float maxRange = 30;
            public float sightAngle = 90;
            public float turnSpeed = 45;

            //Private variables
            Vector3 direction;
            float distance = 0;
            bool inView = false;
            bool inAngle = false;

            #region FrameManagement
            delegate void Frame();
            Frame frame;
            delegate void LateFrame();
            LateFrame lateFrame;
            delegate void LatestFrame();
            LatestFrame latestFrame;

            float lateFr = 15;
            float lateFr_Counter = 0f;

            float latestFr = 30f;
            float latestFr_Counter = 0f;
            #endregion

            public void AIUpdate()
            {
                HandleStates();

                if (frame != null)
                    frame();

                lateFr_Counter++;
                if (lateFr_Counter > lateFr)
                {
                    if (lateFrame != null)
                        lateFrame();

                    lateFr_Counter = 0;
                }

                latestFr_Counter++;
                if (latestFr_Counter > latestFr)
                {
                    if (latestFrame != null)
                        latestFrame();

                    latestFr_Counter = 0;
                }
            }

            void HandleStates()
            {
                switch (state)
                {
                    case AIState.Idle:
                        if(distance > maxRange)
                        {
                            ChangeState(AIState.LateIdle);
                        }
                        else if(distance < lookRange)
                        {
                            ChangeState(AIState.InRange);
                        }
                        break;
                    case AIState.LateIdle:
                        if(distance < maxRange)
                        {
                            ChangeState(AIState.Idle);
                        }
                        break;
                    case AIState.InRange:
                        if(distance > lookRange)
                        {
                            ChangeState(AIState.Idle);
                        }
                        if (inView)
                        {
                            ChangeState(AIState.InView);
                        }
                        break;
                    case AIState.InView:
                        if (!inView)
                        {
                            ChangeState(AIState.InRange);
                        }
                        break;
                    default:
                        break;
                }
            }

            public void ChangeState(AIState newState)
            {
                state = newState;
                frame = null;
                lateFrame = null;
                latestFrame = null;

                switch (newState)
                {
                    case AIState.Idle:
                        lateFrame += IdleBehaviour;
                        break;
                    case AIState.LateIdle:
                        latestFrame += IdleBehaviour;
                        break;
                    case AIState.InRange:
                        lateFrame += InRangeBehaviour;
                        break;
                    case AIState.InView:
                        lateFrame += InRangeBehaviour;
                        frame += InViewBehaviour;
                        break;
                    default:
                        break;
                }
            }

            void IdleBehaviour()
            {
                if (playerTarget == null)
                    return;

                DistanceCheck(playerTarget);
            }

            void InRangeBehaviour()
            {
                if (playerTarget == null)
                    return;

                DistanceCheck(playerTarget);
                GetDirection(playerTarget);
                CheckAngle(playerTarget);
                SightCheck(playerTarget);
            }

            void InViewBehaviour()
            {
                if (playerTarget == null)
                    return;

                GetDirection(playerTarget);
                LookAtTarget();
            }

            void DistanceCheck(Transform target)
            {
                if (target == null)
                    return;

                distance = Vector3.Distance(transform.position, target.position);
            }

            void SightCheck(Transform target)
            {
                if (target == null)
                    return;


                Debug.DrawRay(headBone.position, direction * 100, Color.red, .1f);
                RaycastHit hit;
                if (Physics.Raycast(headBone.position, direction, out hit, lookRange))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.CompareTag("Player"))
                    {
                        inView = true;
                    }
                    else
                        inView = false;
                }
                else
                    inView = false;
            }

            void GetDirection(Transform target)
            {
                if (target == null)
                    return;

                direction = (target.position + Vector3.up - transform.position).normalized;
            }

            void CheckAngle(Transform target)
            {
                if (target == null)
                    return;

                float angle = Vector3.Angle(headBone.forward, direction);

                inAngle = (angle < sightAngle);
            }

            void LookAtTarget()
            {
                if (playerTarget == null)
                    return;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
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
