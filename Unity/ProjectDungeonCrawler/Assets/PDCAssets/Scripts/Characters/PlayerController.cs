using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacter))]
[RequireComponent(typeof(HeadBobComponent))]
public class PlayerController : MonoBehaviour {
    PlayerCharacter motor;
    HeadBobComponent bobComponent;
    [HideInInspector] public Animator anim;

    [SerializeField] Camera cam;
    [SerializeField] public Camera armsCam;

    [Header("PlayerStats")]
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] bool isGrounded = true;

    [Header("GameStats")]
    [SerializeField] float sensitivity = 5f;
    [SerializeField] LayerMask playerLayer;

    [Header("Headbob Stats")]
    [SerializeField] float walkBobSpeed = 1.5f;

    void Start()
    {
        motor = GetComponent<PlayerCharacter>();
        bobComponent = GetComponent<HeadBobComponent>();
        bobComponent.SetupHeadBob(cam);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        Screen.SetResolution(256, 144, true);
    }

    void Update()
    {
        HandleInput();
        HandleChecks();
    }

    float rate = 0;
    void HandleInput()
    {
        #region Movement input
        //Calculate movement
        float _xMove = Input.GetAxisRaw("Horizontal");
        float _yMove = Input.GetAxisRaw("Vertical");

        if (motor.SetMove(_xMove, _yMove))
            bobComponent.BobHead(.05f, walkBobSpeed, false);

        //Calculate player rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0, yRot, 0) * sensitivity;
        motor.SetRotation(rotation);

        //Calculate camera rotation
        float xRot = Input.GetAxisRaw("Mouse Y");
        Vector3 camRotation = new Vector3(xRot, 0, 0) * sensitivity;
        motor.SetCamRotation(-camRotation);

        //Check Jump Input
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                motor.Jump(jumpHeight);
        }
        #endregion

        if (Input.GetButtonDown("Fire3"))
        {
            motor.ThrowWeapon();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            motor.Attack();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            motor.HeavyAttack();
        }
    }

    void HandleChecks()
    {
        //Check if grounded
        if (Physics.Raycast(transform.position + transform.up, -transform.up, 1.3f, playerLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
