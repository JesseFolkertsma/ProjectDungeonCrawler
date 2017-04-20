using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(HeadBobComponent))]
public class PlayerController : MonoBehaviour {
    PlayerMovement movement;
    HeadBobComponent bobComponent;
    [HideInInspector] public Animator anim;

    [SerializeField] Camera cam;
    [SerializeField] public Camera armsCam;

    [Header("PlayerStats")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] bool isGrounded = true;

    [Header("GameStats")]
    [SerializeField] float sensitivity = 5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask interactableLayer;

    [Header("Headbob Stats")]
    [SerializeField] float walkBobSpeed = 1.5f;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        bobComponent = GetComponent<HeadBobComponent>();
        bobComponent.SetupHeadBob(cam);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
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

        anim.SetFloat("Walk", _yMove);
        if (movement.SetMove(_xMove, _yMove))
            bobComponent.BobHead(.05f, walkBobSpeed, false);

        //Calculate player rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0, yRot, 0) * sensitivity;
        movement.SetRotation(rotation);

        //Calculate camera rotation
        float xRot = Input.GetAxisRaw("Mouse Y");
        Vector3 camRotation = new Vector3(xRot, 0, 0) * sensitivity;
        movement.SetCamRotation(-camRotation);

        //Check Jump Input
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
                movement.Jump(jumpHeight);
        }
        #endregion

        if (Input.GetButtonDown("Fire1"))
        {
            anim.ResetTrigger("HeavyAttack");
            anim.SetTrigger("LightAttack");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            anim.ResetTrigger("LightAttack");
            anim.SetTrigger("HeavyAttack");
        }
    }

    void HandleChecks()
    {
        //Check if grounded
        if (Physics.Raycast(transform.position + transform.up, -transform.up, 1.1f, playerLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void SetArms(bool _b)
    {
        armsCam.gameObject.SetActive(_b);
    }
}
