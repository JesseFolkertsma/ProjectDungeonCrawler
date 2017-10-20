using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedController : NetworkBehaviour
{ //Public variables
    public bool isEnabled;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public AudioSource audioS;
    public Transform camHolder;
    public Camera playerCam;
    public HeadBobVariables headbobVariables;
    public LayerMask playerLayer;
    public LayerMask blockPath;
    public float movementSpeed;
    public float acceleration;
    public float movementModifier;
    public float mouseSensitivity;
    public float jumpForce;
    public float maxSlope = 130;
    public float playerLength = 1f;
    public float playerLengthCrouched = .5f;
    public AudioClip[] footSteps;

    //Private variables
    RaycastHit feethit;
    Vector3 direction;
    float bobY;
    float bobX;
    float moveValue;
    bool bobUp;
    bool obstacle;
    bool onSurface;
    bool crouching;

    //Hidden public variables
    [HideInInspector]
    public float acc;
    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public float xInput;
    [HideInInspector]
    public float yInput;

    [Serializable]
    public struct HeadBobVariables
    {
        public bool enableCameraEffects;
        public bool mikeMode;
        [Header("The intensity of headbobbing")]
        public Vector2 headbobIntensity;
        [Header("The speed of headbobbing")]
        public float maxBobSpeed;
        [Header("The panning you get when strafing")]
        public float maxCameraPan;
        [Header("How fast the camera pans")]
        public float panSpeed;
        [Header("Standard camera FOV")]
        public float baseFOV;
        [Header("Extra FOV when running")]
        public float fovBonus;
        [Header("Interpolate speed between FOV")]
        public float changeFOVSpeed;
    }

    bool IsInAngle
    {
        get
        {
            if (maxSlope >= GroundAngle()) return true;
            else return false;
        }
    }

    float pLength
    {
        get
        {
            if (crouching)
            {
                return playerLengthCrouched;
            }
            else
            {
                return playerLength;
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioS = GetComponent<AudioSource>();

        if (headbobVariables.mikeMode)
        {
            headbobVariables.headbobIntensity = new Vector2(.1f, .5f);
            headbobVariables.maxBobSpeed = 7;
            headbobVariables.maxCameraPan = 60;
            headbobVariables.fovBonus = 50;
        }
    }

    void Update()
    {
        if (isEnabled)
        {
            CheckInput();
            Checks();
            CameraEffects();
            GroundAngle();
            HandleCrouching();
            Debug.DrawRay(transform.position + transform.up, Forward() * 3);
        }
    }

    void FixedUpdate()
    {
        if (isEnabled)
        {
            Move();
        }
    }

    public void Disable()
    {
        isEnabled = false;
        FindObjectOfType<ClampedCamera>().isEnabled = false;
    }

    public void Enable()
    {
        isEnabled = true;
        FindObjectOfType<ClampedCamera>().isEnabled = true;
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

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButton("Crouch"))
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }
    }

    void Checks()
    {
        RaycastHit ground;
        if (Physics.SphereCast(transform.position + Vector3.up, 0.20f, Vector3.down, out ground, pLength + 0.1f, playerLayer) && IsInAngle)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        if (Physics.SphereCast(transform.position + Vector3.up, 0.20f, Vector3.down, out feethit, pLength + 0.2f, playerLayer))
        {
            onSurface = true;
        }
        else
        {
            onSurface = false;
        }

        if (direction != Vector3.zero)
        {
            if (Physics.Raycast(transform.position + Vector3.up, direction, .3f, blockPath) || Physics.Raycast(transform.position + Vector3.up / 2, direction, .3f, blockPath))
            {
                obstacle = true;
            }
            else
            {
                obstacle = false;
            }
        }
    }

    void CameraEffects()
    {
        //HeadBobbing
        if (direction != Vector3.zero && grounded)
        {
            Bobhead(1f);
        }

        if (headbobVariables.enableCameraEffects)
        {
            //Stafe effects
            if (xInput != 0)
            {
                Quaternion newRot = camHolder.rotation;
                newRot.eulerAngles += new Vector3(0, 0, headbobVariables.maxCameraPan * -xInput);
                playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, newRot, headbobVariables.panSpeed * Time.deltaTime);
            }
            else
            {
                playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, camHolder.rotation, headbobVariables.panSpeed * Time.deltaTime);
            }
            //Walking forward effects
            if (yInput > 0.1)
            {
                //FOV effect
                playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, headbobVariables.baseFOV + headbobVariables.fovBonus / movementModifier * acc, headbobVariables.changeFOVSpeed * Time.deltaTime);
            }
            else
            {
                if (playerCam.fieldOfView != headbobVariables.baseFOV)
                    playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, headbobVariables.baseFOV, headbobVariables.changeFOVSpeed * 3 * Time.deltaTime);

                if (playerCam.transform.localPosition != Vector3.zero)
                    playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, Vector3.zero, headbobVariables.maxBobSpeed * Time.deltaTime);
            }
        }
    }

    void Bobhead(float multiplier)
    {
        //Calculate bob position
        if (bobUp)
        {
            bobY = headbobVariables.headbobIntensity.y * multiplier;
            if (playerCam.transform.localPosition.y >= bobY - .01)
            {
                bobUp = false;
                Step();
            }
        }
        else
        {
            bobY = -headbobVariables.headbobIntensity.y * multiplier;
            if (playerCam.transform.localPosition.y <= bobY + .01)
            {
                bobUp = true;
                bobX = UnityEngine.Random.Range(-headbobVariables.headbobIntensity.x, headbobVariables.headbobIntensity.x) * multiplier;
                Step();
            }
        }

        //Apply bob position
        Vector3 newPos = new Vector3(bobX, bobY, 0);
        playerCam.transform.localPosition = Vector3.MoveTowards(playerCam.transform.localPosition, newPos, (Time.deltaTime * headbobVariables.maxBobSpeed / movementModifier * acc) * multiplier);
    }

    void HandleCrouching()
    {
        if (crouching)
        {
            GetComponent<Animator>().SetBool("Crouch", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("Crouch", false);
        }
    }

    void Step()
    {
        if (grounded)
        {
            if (audioS != null && footSteps.Length > 0)
            {
                audioS.clip = footSteps[UnityEngine.Random.Range(0, footSteps.Length)];
                audioS.Play();
            }
        }
    }

    void Jump()
    {
        if (grounded)
            rb.velocity += Vector3.up * jumpForce;
    }

    Vector3 Forward()
    {
        if (!onSurface) return transform.forward;
        return Vector3.Cross(feethit.normal, -transform.right);
    }

    float GroundAngle()
    {
        if (!onSurface) return 90;
        return Vector3.Angle(feethit.normal, direction);
    }

    public void Move()
    {
        if (direction != Vector3.zero && !obstacle && IsInAngle)
        {
            acc = Mathf.Lerp(acc, movementModifier, Time.fixedDeltaTime * acceleration);
            Vector3 newDir = new Vector3(direction.x, Forward().y, direction.z);
            rb.MovePosition(rb.position + (newDir * Time.fixedDeltaTime * movementSpeed * acc));
        }
        else
        {
            acc = Mathf.Lerp(acc, 0f, Time.fixedDeltaTime * acceleration * 2);
        }
    }
}
