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
    public Animator anim;
    public BanditIK ik;
    public Transform camHolder;
    public Camera playerCam;
    public HeadBobVariables headbobVariables;
    public CollisionVariables collisionVariables;
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
    public float playerFeetThickness = .2f;
    public AudioClip[] footSteps;
    [HideInInspector]
    public Transform rightIKPos;
    public Transform leftIKPos;

    //Private variables
    RaycastHit feethit;
    Vector3 direction;
    float bobY;
    float bobX;
    float moveValue;
    bool canMove;
    bool canLook;
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
    [HideInInspector, SyncVar]
    public bool sGrounded;
    [HideInInspector, SyncVar]
    public float sxInput;
    [HideInInspector, SyncVar]
    public float syInput;

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

    [Serializable]
    public struct CollisionVariables
    {
        public float blockRayLenght;
        public float blockRayThickness;
        public RaycastHit colHitInfo;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioS = GetComponent<AudioSource>();
        canMove = true;
        canLook = true;

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
        if (isLocalPlayer)
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
        else
        {
            HandleAnimations();
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (isEnabled)
            {
                Move();
            }
        }
    }
    
    /// <summary>
    /// Enable or disable controls
    /// </summary>
    /// <param name="value">Enable or disable</param>
    /// <param name="movement">Apply on movement</param>
    /// <param name="camera">Apply on rotation and camera controls</param>
    public void EnableControls(bool value, bool movement, bool camera)
    {
        if (movement)
        {
            canMove = value;
        }
        if (camera)
        {
            canLook = value;
        }
    }

    public void HandleAnimations()
    {
        if(anim != null)
        {
            anim.SetFloat("MoveX", sxInput);
            anim.SetFloat("MoveY", syInput);
            anim.SetBool("IsFalling", !sGrounded);

            if (rightIKPos != null)
            {
                ik.SetRightIKPosition(rightIKPos);
            }

            if (leftIKPos != null)
            {
                ik.SetLeftIKPosition(leftIKPos);
            }
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
        CmdSetX(Input.GetAxisRaw("Horizontal"));
        xInput = Input.GetAxisRaw("Horizontal");
        CmdSetY(Input.GetAxisRaw("Vertical"));
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
        if (Physics.SphereCast(transform.position + Vector3.up, playerFeetThickness, Vector3.down, out ground, pLength + 0.1f, playerLayer) && IsInAngle)
        {
            grounded = true;
            CmdSetGrounded(true);
        }
        else
        {
            grounded = false;
            CmdSetGrounded(false);
        }
        if (Physics.SphereCast(transform.position + Vector3.up, playerFeetThickness, Vector3.down, out feethit, pLength + 0.2f, playerLayer))
        {
            onSurface = true;
        }
        else
        {
            onSurface = false;
        }

        if (direction != Vector3.zero)
        {
            if (Physics.SphereCast(camHolder.position, collisionVariables.blockRayThickness, direction, out collisionVariables.colHitInfo, collisionVariables.blockRayLenght, blockPath) || Physics.SphereCast(camHolder.position - (Vector3.up * (pLength / 1.5f)), collisionVariables.blockRayThickness, direction, out collisionVariables.colHitInfo, collisionVariables.blockRayLenght, blockPath)) 
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

    [Command]
    void CmdSetGrounded(bool set)
    {
        RpcSetGrounded(set);
    }

    [ClientRpc]
    void RpcSetGrounded(bool set)
    {
        sGrounded = set;
    }

    [Command]
    void CmdSetX(float x)
    {
        RpcSetX(x);
    }

    [ClientRpc]
    void RpcSetX(float x)
    {
        sxInput = x;
    }

    [Command]
    void CmdSetY(float y)
    {
        RpcSetY(y);
    }

    [ClientRpc]
    void RpcSetY(float y)
    {
        syInput = y;
    }
}
