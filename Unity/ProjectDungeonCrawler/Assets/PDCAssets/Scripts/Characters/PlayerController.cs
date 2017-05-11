using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Characters
{
    public class PlayerController : BaseCharacter
    {
        public static PlayerController instance;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        void Update()
        {
            CheckInput();
            Move();
        }

        void CheckInput()
        {
            #region MovementInput
            float xInput = Input.GetAxisRaw("Horizontal");
            float yInput = Input.GetAxisRaw("Vertical");


            #endregion
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
}
