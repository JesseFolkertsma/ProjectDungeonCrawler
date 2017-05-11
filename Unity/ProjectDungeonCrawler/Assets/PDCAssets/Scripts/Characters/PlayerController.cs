using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDC.Characters
{
    public class PlayerController : BaseCharacter
    {
        public static PlayerController instance;

        //Public variables
        public Rigidbody rb;
        public Transform camHolder;
        public Camera playerCam;
        public HeadBobVariables headbobVariables;
        public float movementSpeed;
        public float mouseSensitivity;

        //Private variables
        Vector3 direction;
        Vector3 mouseX;
        Vector3 mouseY;
        float xInput;
        float yInput;
        float xRot;
        float yRot;

        [Serializable]
        public struct HeadBobVariables
        {
            public float maxBobSpeed;
            public float maxCameraPan;
            public float panSpeed;
            public float baseFOV;
            public float maxFOV;
            public float changeFOVSpeed;
        }

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            CheckInput();
            CameraEffects();
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        void CheckInput()
        {   
            #region MovementInput
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
            Vector3 xVector = transform.right * xInput;
            Vector3 yVector = transform.forward * yInput;
            direction = (xVector + yVector).normalized;
            #endregion

            #region RotationInput
            xRot = Input.GetAxisRaw("Mouse X");
            yRot = Input.GetAxisRaw("Mouse Y");
            mouseX = Vector3.up * xRot;
            mouseY = -(Vector3.right * yRot);
            #endregion
        }

        void CameraEffects()
        {
            if(xInput != 0)
            {
                playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, camHolder.rotation, headbobVariables.panSpeed * Time.deltaTime);
            }
            else
            {
                playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, camHolder.rotation, headbobVariables.panSpeed * Time.deltaTime);
            }
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
            if(direction != Vector3.zero)
            {
                rb.MovePosition(rb.position + (direction * Time.fixedDeltaTime * movementSpeed));
            }
        }

        void Rotate()
        {
            if(mouseX != Vector3.zero)
            {
                transform.Rotate(mouseX * Time.fixedDeltaTime * mouseSensitivity);
            }

            if(camHolder != null)
            {
                if(mouseY != Vector3.zero)
                {
                    camHolder.Rotate(mouseY * Time.fixedDeltaTime * mouseSensitivity);
                }
            }
        }
    }
}
