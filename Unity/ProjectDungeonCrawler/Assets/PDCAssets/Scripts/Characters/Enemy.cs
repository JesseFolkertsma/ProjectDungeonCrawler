using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Characters;
using System;

namespace PDC.Characters {

    public class Enemy : BaseCharacter
    {
        

        public class EnemyManagement
        {
            public float fastUpdate = 0.1f, slowUpdate = 1; //speed of updating, depending how far the player is from this enemy
            public int fastDis, slowDis; //lerp between ^ this with percentage distance
        }

        public EnemyManagement enemy;

        protected virtual void Awake()
        {
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
            float updateTime = enemy.slowUpdate;
            //func idle, default case for most ai
            while (loopIdle)
            {
                //check if able to see player {
                //check if able to attack
                //else
                //check if close enough to move }

                //else
                //func searchforplayer

                //ienumerator that balances checks per second on distance to player
                //update yield return value seconds
                yield return new WaitForSeconds(updateTime);
            }
        }

        //if !close distance return till next check

        protected virtual bool CheckIfAbleToAttack()
        {
            //check plz
            return true;
        }

        public override void Attack()
        {
            //shoot raycasts
        }

        public override void Die()
        {
            //drop items, calc which ones
        }

        public override void Move()
        {
            //flying / grounded are both a thing
        }
    }
}
