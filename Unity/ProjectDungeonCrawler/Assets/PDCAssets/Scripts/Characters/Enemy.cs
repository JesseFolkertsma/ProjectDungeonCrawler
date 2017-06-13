using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;
using UnityEngine.AI;

namespace PDC.Characters {
    [RequireComponent(typeof(MoveManager))]
    public class Enemy : BaseCharacter
    {
        private Rigidbody rb;

        [Serializable]
        public class EnemyStats
        {
            public int damage;
        }
        public EnemyStats enemyStats;
        protected static PlayerReference pC = null;
        protected MoveManager navAgent;
        public enum Status {Idle, Attacking, Moving}
        public Status status = Status.Idle;
        [SerializeField]
        private float recalculatePathTime = 1;
        [SerializeField]
        private int nodeStoppingDistance = 3;
        private float widthNode;

        public class PlayerReference
        {
            public PlayerCombat playerController;
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

            public PlayerReference(PlayerCombat _pC)
            {
                playerController = _pC;
                transform = _pC.transform;
            }
        }

        [SerializeField, Tooltip("Used for scanning player")]
        private float heightChar, widthChar;
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

        [Serializable]
        public class EnemyManagement
        {
            public float fastUpdate = 0.1f, slowUpdate = 1; //speed of updating, depending how far the player is from this enemy
            public float fastDis, slowDis, engagementRange, attackRange; //lerp between ^ this with percentage distance
        }

        public EnemyManagement enemy;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            SetupNavMesh();
            StartCoroutine(SearchForPlayer());
        }

        private void Start()
        {
            widthNode = PathFinding.self.widthSizeNode;
        }

        private void SetupNavMesh()
        {
            navAgent = GetComponent<MoveManager>();
        }

        protected IEnumerator SearchForPlayer()
        {
            while(!(pC != null)) //makes sure there is a playercontroller reference
            {
                if (PlayerCombat.instance != null)
                    pC = new PlayerReference(PlayerCombat.instance);
                yield return null;
            }

            while (!PathFinding.pathfindable)
                yield return null;

            StartIdle();
        }

        protected void StartIdle()
        {
            StartCoroutine(Idle());
        }

        [HideInInspector]
        public bool loopIdle = true;
        private float updateTime;
        protected virtual IEnumerator Idle()
        {
            updateTime = enemy.slowUpdate;
            float playerDistance = 0;
            //func idle, default case for most ai
            while (loopIdle)
            {
                //check distance
                playerDistance = GetPlayerDistance();

                //while moving
                while (status == Status.Moving)
                {
                    CalcRefreshRate();
                    if (CheckIfAbleToAttack())
                        //check if able to attack
                        Attack();
                    else
                    {
                        Move(pC.Position); //because it also calculates new player position
                    }

                    yield return new WaitForSeconds(recalculatePathTime * GetRefreshPercentage());
                }

                //while attacking
                while (status == Status.Attacking)
                {
                    CalcRefreshRate();
                    yield return new WaitForSeconds(updateTime);
                }

                //check in range
                //if so move
                CalcRefreshRate();
                PauseMovement();
                if (playerDistance <= enemy.engagementRange)
                    if (CheckIfSeePlayer())
                    {
                        Move(pC.Position);
                    }
                yield return new WaitForSeconds(updateTime);
            }
        }

        //optimization
        private void CalcRefreshRate()
        {
            //calculate new updatetime
            //ienumerator that balances checks per second on distance to player
            updateTime = Mathf.Lerp(enemy.fastUpdate, enemy.slowUpdate, GetRefreshPercentage());
        }

        private float GetRefreshPercentage()
        {
            float percentage = Mathf.InverseLerp(enemy.fastDis, enemy.slowDis, GetPlayerDistance());
            if (percentage > 100)
                percentage = 100;
            return percentage;
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
            bool ret = GetPlayerDistance() < enemy.attackRange;
            //check attack info, check if close enough        
            return ret;
        }

        public void Attack()
        {
            //shoot raycasts and get direction, and rotate that way
            PauseMovement();
            status = Status.Attacking;
            print("Attacking!");
        }

        public override void Die()
        {
            //drop items, calc which ones in enemymanager
        }

        public void Move(Vector3 target)
        {
            navAgent.MoveTowards(target, _Move);
            status = Status.Moving;
        }

        public void _Move(List<Vector3> path)
        {
            if (!(path != null))
                return;
            if (path.Count == 0)
                return;
            _path = path;

            if (moving != null)
                StopCoroutine(moving);
            moving = StartCoroutine(MoveCoroutine());
        }

        private Coroutine moving;
        private List<Vector3> _path;
        private Vector3 direction;
        private IEnumerator MoveCoroutine()
        {
            List<Vector3> rest = _path;
            status = Status.Moving;
            Vector3 destination;

            while (rest.Count > 0)
            {
                destination = rest[rest.Count - 1];
                while (Vector3.Distance(transform.position, rest[0]) < Vector3.Distance(destination, rest[0]) + nodeStoppingDistance * widthNode)
                {
                    rest.Remove(destination);
                    if (rest.Count == 0)
                    {
                        status = Status.Idle;
                        yield break;
                    }

                    destination = rest[rest.Count - 1];
                }
                
                direction = (transform.position - destination).normalized;
                rb.MovePosition(transform.position - direction * Time.deltaTime * characterStats.movementSpeed);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            //finally end moving and set status to idle
            status = Status.Idle;
            yield break;
        }

        protected virtual void PauseMovement()
        {
            if (moving != null)
                StopCoroutine(moving);
            status = Status.Idle;
        }
    }
}
