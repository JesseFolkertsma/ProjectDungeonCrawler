using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;
using UnityEngine.AI;

using PDC;
using PDC.StatusEffects;

namespace PDC.Characters {
    [RequireComponent(typeof(MoveManager), typeof(TagManager))]
    public class Enemy : BaseCharacter, IHitable
    {
        //rotate towards player while moving not working
        //kleine hiccup als hij aan het eind van zn tijdelijke path is
        //mapmode movement
        //other shizzle, look at planning

        private Rigidbody rb;
        [SerializeField]
        private Animator anim;

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
                    if (transform == null)
                        return Vector3.zero;
                    return transform.position;
                }
            }
            public GameObject GameObject
            {
                get
                {
                    if (transform == null)
                        return null;
                    return transform.gameObject;
                }
            }

            public PlayerReference(PlayerCombat _pC)
            {
                playerController = _pC;
                transform = _pC.transform;
            }
        }

        private static float heightChar = 0.8f, widthChar = 0.2f;
        protected List<Vector3> GetMultiPlayerPos()
        {
            if (pC == null)
                return null;
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

        private void OnSceneExit()
        {
            pC = null;
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
            while(pC == null) //makes sure there is a playercontroller reference
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
            if (pC != null)
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
                        if (!CheckIfAbleToAttack())
                            Move(pC.Position); //because it also calculates new player position

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
                            if (!CheckIfAbleToAttack())
                                Move(pC.Position);

                    yield return new WaitForSeconds(updateTime);
                }
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
            if (pC == null)
                return 0;
            return Vector3.Distance(transform.position, pC.Position);
        }

        protected virtual bool CheckIfSeePlayer()
        {
            if (pC == null)
                return false;
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
            float dis = GetPlayerDistance();
            bool ret = dis < enemy.attackRange;
            //check attack info, check if close enough    
            if (!ret)
                return ret;

            EnemyAttack attack = null;
            //check if attacks are in range and choose attack which has the least range
            foreach (EnemyAttack eA in enemyAttacks)
            {
                if (eA.range > dis)
                {
                    if (attack != null)
                    {
                        if (eA.range < attack.range)
                            attack = eA;
                    }
                    else
                        attack = eA;
                }
            }

            if(attack != null)
            {
                UseAttack(attack);
                return true;
            }
                
            return false;
        }

        public int dropChanceItem = 30;
        public GameObject[] itemsToDrop;
        public GameObject coin;

        public override void Die()
        {
            if (isdead)
                return;
            isdead = true;
            //drop items, calc which ones in enemymanager
            Generating.MapVisualizer.self.OnEnemyDeath(transform);
            GameObject ragD = Instantiate(ragdoll, transform.position, transform.rotation);
            ragD.GetComponent<Animator>().runtimeAnimatorController = anim.runtimeAnimatorController;
            Vector3 dir = (transform.position - PlayerCombat.instance.transform.position).normalized;
            ragD.GetComponentInChildren<Rigidbody>().AddForce(dir * 5000);
            if(UnityEngine.Random.Range(0,100) < dropChanceItem)
            {
                GameObject drop = Instantiate(itemsToDrop[UnityEngine.Random.Range(0, itemsToDrop.Length)], ObjectCenter, transform.rotation);
                drop.GetComponent<Rigidbody>().AddForce(Vector3.up * 500);
            }
            int rng = UnityEngine.Random.Range(0, 100);
            for (int i = 0; i < rng; i++)
            {
                Instantiate(coin, ObjectCenter, transform.rotation);
            }
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
                Vector3 origin = transform.position;
                origin.y += raycastOffsetHeight;
                return origin;
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
                if(pC != null)
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

        #region Rotation

        private Vector3 targetDir, newDir;
        private float step;
        private void Rotate(Vector3 des)
        {
            targetDir = (des - transform.position).normalized;
            step = rotateSpeed * Time.deltaTime;
            newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            //Debug.DrawRay(transform.position, newDir, Color.red);
            Quaternion rot = transform.rotation;
            rot.y = Quaternion.LookRotation(newDir).y;
            transform.rotation = rot;
        }

        private Coroutine rotateCoroutine;
        private void RotateContinual(Transform t)
        {
            EndRotateContinual();
            rotateCoroutine = StartCoroutine(_RotateContinual(t));
        }

        private void EndRotateContinual()
        {
            if (rotateCoroutine != null)
                StopCoroutine(rotateCoroutine);
        }

        private IEnumerator _RotateContinual(Transform t)
        {
            while (true)
            {
                Rotate(t.position);
                yield return null;
            }
        }

        #endregion

        #region Combat

        [Serializable]
        public class EnemyAttack
        {
            public int index;
            public int damage;
            public float range;
            public EffectType type;
            public StatusEffect[] statusEffects;
            public bool ranged;
        }

        [SerializeField]
        private EnemyAttack[] enemyAttacks;
        private EnemyAttack curAttack;
        [HideInInspector]
        public bool damages;
        private List<IHitable> hits = new List<IHitable>(); //to make sure he doesnt hit something twice in one attack
        private Transform target;

        public void SwitchDamageFrames()
        {
            damages = !damages;
        }

        protected virtual void UseAttack(EnemyAttack eA)
        {
            if (pC == null)
                return;
            PauseMovement(true);
            status = Status.Attacking;
            target = pC.transform;

            curAttack = eA;
            RotateContinual(pC.transform);

            damages = false;
            hits.Clear();
            anim.SetInteger(attackAnim, eA.index);
        }

        public virtual void EndAttack()
        {
            //continue while still in range
            if (GetPlayerDistance() <= curAttack.range)
                return;
            anim.SetInteger(attackAnim, 0);
            EndRotateContinual();
            status = Status.Idle;
        }

        [SerializeField]
        private float secondsUntilRangedHit;
        [SerializeField]
        private Transform originRangeAttacks;
        public virtual void RangedHit(int hitChance)
        {
            int index = UnityEngine.Random.Range(0,100);
            List<Vector3> positions = GetMultiPlayerPos();

            if (hitChance < index)
                index = UnityEngine.Random.Range(1, positions.Count);
            else
                index = 0;

            StartCoroutine(_RangedHit(positions[index]));
        }

        private IEnumerator _RangedHit(Vector3 hitPoint)
        {
            yield return new WaitForSeconds(secondsUntilRangedHit);

            RaycastHit hit;

            if(Physics.Raycast(originRangeAttacks.position, hitPoint, out hit, curAttack.range))
            {
                print("i hit: " + hit.transform.name);
                IHitable iHit = (IHitable)hit.transform.GetComponent(typeof(IHitable));
                if (iHit != null)
                    HitRangeObject(iHit);
            }
        }

        public virtual void HitRangeObject(IHitable hit)
        {
            if (hit as Enemy != null)
                return;
            //check if attacking     
            hits.Add(hit);
            hit.GetHit(curAttack.damage, curAttack.type, curAttack.statusEffects, transform.position);
        }

        public virtual void HitObject(IHitable hit)
        {
            if (hit as Enemy != null)
                return;
            if (hits.Contains(hit))
                return;
            if (status != Status.Attacking)
                return;
            if (curAttack.ranged)
                return;
            if (!damages)
                return;

            //check if attacking     
            hits.Add(hit);      
            hit.GetHit(curAttack.damage, curAttack.type, curAttack.statusEffects, transform.position);
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
