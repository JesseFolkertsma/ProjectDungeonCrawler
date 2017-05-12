using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;
using UnityEngine.AI;

namespace PDC.Characters {

    public class Enemy : BaseCharacter
    {
        protected static PlayerReference pC;
        protected NavMeshAgent navAgent;
        public enum Status {Idle, Yield, Attacking}
        public Status status = Status.Yield;

        public class PlayerReference
        {
            public PlayerController playerController;
            //room for other scripts
            public Transform transform;
            public Vector3 Position
            {
                get
                {
                    return transform.position;
                }
            }
            public GameObject GameObject
            {
                get
                {
                    return transform.gameObject;
                }
            }

            public PlayerReference(PlayerController _pC)
            {
                playerController = _pC;
                transform = _pC.transform;
            }
        }

        [SerializeField, Tooltip("Used for scanning of player")]
        private int heightChar, widthChar;
        protected List<Vector3> GetMultiPlayerPos()
        {
            //get normal, mid, left mid / right mid, high
            List<Vector3> ret = new List<Vector3>();
            ret.Add(pC.Position);

            //neutral mid
            Vector3 mid = pC.Position;
            mid.y += heightChar / 2;
            ret.Add(mid);
            //left mid
            mid.x -= widthChar / 2;
            ret.Add(mid);
            //right mid
            mid.x += widthChar;
            ret.Add(mid);
            //high
            mid = pC.Position;
            mid.y += heightChar;
            ret.Add(mid);

            return ret;
        }

        public class EnemyManagement
        {
            public float fastUpdate = 0.1f, slowUpdate = 1; //speed of updating, depending how far the player is from this enemy
            public int fastDis, slowDis, engagementRange; //lerp between ^ this with percentage distance
        }

        public EnemyManagement enemy;

        protected virtual void Awake()
        {
            SearchForPlayer();
        }

        protected IEnumerator SearchForPlayer()
        {
            while((pC != null)) //makes sure there is a playercontroller reference
            {
                if (PlayerController.instance != null)
                    pC = new PlayerReference(PlayerController.instance);
                yield return null;
            }

            StartIdle();
        }

        protected void StartIdle()
        {
            StartCoroutine(Idle());
        }

        [HideInInspector]
        public bool loopIdle = true;
        protected virtual IEnumerator Idle()
        {
            Coroutine followPlayer = null;
            float updateTime = enemy.slowUpdate;
            float playerDistance = 0;
            //func idle, default case for most ai
            while (loopIdle)
            {
                while (status == Status.Attacking)
                    yield return new WaitForSeconds(updateTime);
                status = Status.Idle;
                //check distance
                playerDistance = GetPlayerDistance();  
                if (playerDistance <= enemy.engagementRange)
                {
                    //search for player
                    if (CheckIfSeePlayer())
                    {
                        //check if able to attack
                        if (CheckIfAbleToAttack())
                            Attack();
                        //check if close enough to move
                        else
                            Move();
                    }
                    else if (!(followPlayer != null))
                        followPlayer = StartCoroutine(FollowingPlayer());
                }

                //calculate new updatetime
                //ienumerator that balances checks per second on distance to player
                float percentage = Mathf.InverseLerp(enemy.fastDis, enemy.slowDis, playerDistance);
                if (percentage > 100)
                    percentage = 100;
                updateTime = Mathf.Lerp(enemy.fastUpdate, enemy.slowUpdate, percentage);

                //update yield return value seconds
                status = Status.Yield;
                yield return new WaitForSeconds(updateTime);
            }
        }

        [SerializeField]
        private int chaseTime = 5;
        private float _chaseTime;
        protected virtual IEnumerator FollowingPlayer()
        {
            _chaseTime = chaseTime;
            while(_chaseTime > 0)
            {
                _chaseTime -= Time.deltaTime;
                yield return null;
            }
            _chaseTime = 0;
        }

        protected float GetPlayerDistance()
        {
            return Vector3.Distance(transform.position, pC.Position);
        }

        protected virtual bool CheckIfSeePlayer()
        {
            /*
            so this is a weird check
            since the player has no collider whatsoever, im going to pass the "see player"
            if at least one of the raycasts DOESNT hit anything
            */
            foreach (Vector3 pos in GetMultiPlayerPos())
                if(!Physics.Linecast(transform.position, pos))
                    return true;
            return false;
        }

        protected virtual bool CheckIfAbleToAttack()
        {
            if (status != Status.Idle)
                return false;
            //check attack info, check if close enough        
            return true;
        }

        public override void Attack()
        {
            status = Status.Attacking;
            //shoot raycasts and get direction, and rotate that way
            print("Attacking!");
        }

        public override void Die()
        {
            //drop items, calc which ones
        }

        public override void Move()
        {
            //flying / grounded are both a thing, just move to player boi
            navAgent.Move(pC.Position);
        }
    }
}
