using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;
using UnityEngine.AI;

using PDC;
using PDC.StatusEffects;

namespace PDC.Characters {
    [RequireComponent(typeof(MoveManager))]
    public class Enemy : BaseCharacter, IHitable
    {
        //rotate towards player while moving not working
        //kleine hiccup als hij aan het eind van zn tijdelijke path is
        //mapmode movement
        //other shizzle, look at planning

        private Rigidbody rb;
        [SerializeField]
        private Animator anim;

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

        [SerializeField]
        private GameObject ragdoll;

        [SerializeField]
        private string walkAnim = "Moving", walkAnimValue = "Movespeed", attackAnim = "Attack";

        [SerializeField]
        private float raycastOffsetHeight = 1.5f;

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

        private static float heightChar = 2, widthChar = 1;
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
            public float fastDis = 10, slowDis = 30, engagementRange = 20, attackRange = 1; //lerp between ^ this with percentage distance
        }

        public EnemyManagement enemy;

        protected virtual void Awake()
        {
            PrepareSelf();
            SetupNavMesh();
            StartCoroutine(SearchForPlayer());
        }

        private void PrepareSelf()
        {
            rb = GetComponent<Rigidbody>();
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
                PauseMovement(true);

                if (playerDistance <= enemy.engagementRange)
                    if (CheckIfSeePlayer())
                        Move(pC.Position);

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
            Vector3 origin = transform.position;
            origin.y += raycastOffsetHeight;

            RaycastHit hit;
            foreach (Vector3 pos in GetMultiPlayerPos())
            {
                //Debug.DrawLine(origin, pos, Color.red, 1);
                if (Physics.Linecast(origin, pos, out hit))
                    if(hit.transform == pC.transform)
                        return true;
            }
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
            PauseMovement(true);
            status = Status.Attacking;
            print("Attacking!");
        }

        public override void Die()
        {
            if (isdead)
                return;
            isdead = true;
            //drop items, calc which ones in enemymanager
            Instantiate(ragdoll, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        public void Move(Vector3 target)
        {
            status = Status.Moving;
            anim.SetBool(walkAnim, true);
            navAgent.MoveTowards(target, _Move);
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

        public Vector3 ObjectCenter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private float walkAnimLerpValue;
        private Vector3 destination, quatRot;
        [SerializeField]
        private float rotateSpeed;
        private IEnumerator MoveCoroutine()
        {
            List<Vector3> rest = _path;
            status = Status.Moving;

            while (rest.Count > 0)
            {
                if (walkAnimLerpValue < 1)
                    walkAnimLerpValue += Time.deltaTime;
                anim.SetFloat(walkAnimValue, walkAnimLerpValue);

                destination = rest[rest.Count - 1];
                while (Vector3.Distance(transform.position, rest[0]) < Vector3.Distance(destination, rest[0]) + nodeStoppingDistance * widthNode)
                {
                    rest.Remove(destination);
                    if (rest.Count == 0)
                    {
                        PauseMovement(true);
                        yield break;
                    }

                    destination = rest[rest.Count - 1];
                }

                //rotate the enemy towards the player
                Rotate(pC.transform.position);

                //move towards player
                direction = (transform.position - destination).normalized;
                rb.MovePosition(transform.position - direction * Time.deltaTime * characterStats.movementSpeed);              

                yield return new WaitForSeconds(Time.deltaTime);
            }

            //finally end moving and set status to idle
            PauseMovement(true);
            yield break;
        }

        private Vector3 targetDir, newDir;
        private float step;
        private void Rotate(Vector3 des)
        {
            targetDir = des - transform.position;
            step = rotateSpeed * Time.deltaTime;
            newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            //Debug.DrawRay(transform.position, newDir, Color.red);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        #region Combat

        protected virtual void UseAttack(int attack)
        {
            UseAttackAnim(attack);
        }

        private void UseAttackAnim(int attack)
        {
            anim.SetInteger(attackAnim, attack);
        }

        protected virtual void EndAttack(int attack)
        {
            EndAttackAnim(attack);
        }

        private void EndAttackAnim(int attack)
        {
            anim.SetInteger(attackAnim, attack);
        }

        #endregion

        protected virtual void PauseMovement(bool stop)
        {
            if (moving != null)
                StopCoroutine(moving);
            bool stopping = stop;
            if (stopping)
            {
                status = Status.Idle;

                anim.SetBool(walkAnim, false);
                anim.SetFloat(walkAnimValue, 0);
            } 
        }

        public void GetHit(float damage, EffectType hitType, StatusEffect[] effects, Vector3 shotPosition)
        {
            Die(); //DIE DIE!
        }
    }
}
