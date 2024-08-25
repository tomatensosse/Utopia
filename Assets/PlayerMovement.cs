using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public Rigidbody playerRigidbody;
    public CapsuleCollider playerCollider;
    public float sprintSpeed = 6.0f;
    public float walkSpeed = 3.0f;
    public float acceleration = 8f;
    private float moveSpeed;
    private float speedMultiplier = 8.0f;
    private float horizontalMovement, verticalMovement;
    private Vector3 moveDirection;
    private Vector3 previousPosition;

    [Header("Checkers")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask whatIsGround;
    private bool isGrounded;
    private bool isStepping;
    private bool isJumping;

    [Header("Slope")]
    public float maxSlopeAngle = 55f;
    private Vector3 slopeMoveDirection;
    private RaycastHit slopeHit;

    [Header("Step")]
    public Transform stepRayLower;
    public float stepHeight = 0.6f;
    public float stepBoost = 0.2f;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 4f;
    public float decelerateDrag = 30.0f;
    public float slopeDrag = 6f;
    private float dragMultiplier = 0.05f;

    [Header("Jump & Walljump")]
    public float jumpForce = 8f;
    public float wallJumpForce = 18f;
    public int maximumWallJumps = 3;
    private int wallJumpsInitiated = 0;
    private RaycastHit wallJumpHit;

    [Header("Camera")]
    public Transform cameraPosition;
    public Transform orientation;
    public float sensX = 2f, sensY = 2f;
    private Camera playerCamera;
    private GameObject cameraContainer;
    private float xRotation, yRotation;
    private float mouseX, mouseY;
    [SyncVar] public float xRotationSync, yRotationSync;

    [Header("Camera Effects")]
    public float cameraSidewaysTilt = 0.01f;
    public float cameraSidewaysTiltTime = 0.05f;
    public float defaultFov = 70f;
    // the camera fov will increase after reaching this speed on rigidbody.velocity.z
    public float increaseFovAfterSpeed = 8f;
    public float fovIncreasePerSpeed = 3f;
    public float maximumFovIncrease = 4f;
    public float fovLerpTime = 0.05f;

    [Header("Animation")]
    public GameObject playerModel;
    public Animator playermodelAnimator;
    public float runAnimationTreshold = 5f;
    
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("External References")]
    public bool inputEnabled = false;

    [Header("Debug")]
    public bool debugLogs = false;
    [ConditionalHide(nameof(debugLogs), true)]
    public bool debugLogSlope, debugLogStep, debugLogVelocity, debugLogInput, 
    debugLogCameraEffects, debugLogPlayerDirection, debugLogCanWallJump,
    debugLogDrag;

    //List<MovementAbility> movementAbilities; TBA!!!

    private void Update()
    {
        if (!isLocalPlayer)
        {
            orientation.rotation = Quaternion.Euler(0, yRotationSync, 0);

            return;
        }

        if (inputEnabled) { MyInput(); }

        UpdateCamera();
        CameraEffect();

        ControlSpeed();
        MovePlayer();
        ControlDrag();
        HandleSlope();
        WallJump();

        //ControlAnimation(); disabled for now because of the lack of the animatorcontroller
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, whatIsGround);

        StepClimb();
    }

    public void InitializePlayer()
    {
        if (isLocalPlayer) { IsLocalPlayer(); }
    }

    private void IsLocalPlayer()
    {
        cameraContainer = GameObject.Find("CameraContainer");
        cameraContainer.GetComponent<MoveCamera>().cameraPosition = cameraPosition;
        cameraContainer.GetComponent<MoveCamera>().active = true;

        playerCamera = cameraContainer.GetComponentInChildren<Camera>();

        playerModel.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateCamera()
    {
        cameraContainer.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        xRotationSync = xRotation;
        yRotationSync = yRotation;
    }

    private void CameraEffect()
    {
        float fov = defaultFov + ((playerRigidbody.velocity.magnitude - increaseFovAfterSpeed) * fovIncreasePerSpeed);
        fov = Mathf.Clamp(fov, defaultFov, fov + maximumFovIncrease);

        // for tilting whilst moving left and right
        float z = horizontalMovement * cameraSidewaysTilt;

        // for assinging each axis
        Quaternion cameraRotation = new Quaternion(0, 0, z, 1);

        // lerp each effect
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, cameraRotation, cameraSidewaysTiltTime);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, fovLerpTime);

        if (debugLogCameraEffects)
        { 
            Debug.Log(Quaternion.Lerp(playerCamera.transform.localRotation, cameraRotation, cameraSidewaysTiltTime));
            Debug.Log(Mathf.Lerp(playerCamera.fieldOfView, fov, fovLerpTime));
        }
    }

    private void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * sensX;
        xRotation -= mouseY * sensY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        // if the player is grounded and not on a slope
        if (isGrounded && !OnSlope(transform.position).isOnSlope)
        {
            if (!isStepping) { playerRigidbody.useGravity = true; }
            playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * speedMultiplier, ForceMode.Acceleration);
        }
        // if the player is on a slope and can climb it
        else if (isGrounded && OnSlope(transform.position).canClimpSlope)
        {
            if (!isStepping) { playerRigidbody.useGravity = false; }
            playerRigidbody.AddForce(slopeMoveDirection.normalized * moveSpeed * speedMultiplier, ForceMode.Acceleration);
        }
        // if the player is on a slope and can't climb it
        else if (isGrounded && OnSlope(transform.position).isOnSlope)
        {
            if (!isStepping) { playerRigidbody.useGravity = true; }
        }
        // if the player is in the air
        else if (!isGrounded)
        {
            if (!isStepping) { playerRigidbody.useGravity = true; }
            playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * speedMultiplier, ForceMode.Acceleration);
        }

        if (debugLogVelocity) { Debug.Log("Player velocity: " + playerRigidbody.velocity); }
    }

    // Controls the player speed based on the player input
    private void ControlSpeed()
    {
        if (Input.GetKey(sprintKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration);
        }
    }

    private void ControlDrag()
    {
        if (isGrounded && !isMovingControlled() && !OnSlope(transform.position).isOnSlope && !isJumping)
        {
            if (debugLogDrag) { Debug.Log("Drag :: Decelerating on ground"); }
            
            var v = playerRigidbody.velocity;
            v.y = 0f;
            v = -v * v.magnitude;

            playerRigidbody.AddForce(decelerateDrag * dragMultiplier * v, ForceMode.Force);
        }
        else if (isGrounded && !isMovingControlled() && OnSlope(transform.position).isOnSlope && !isJumping)
        {
            if (debugLogDrag) { Debug.Log("Drag :: Decelerating on slope"); }
            
            var v = playerRigidbody.velocity;
            v = -v * v.magnitude;

            playerRigidbody.AddForce(decelerateDrag * dragMultiplier * v, ForceMode.Force);
        }
        else if (isGrounded && !OnSlope(transform.position).canClimpSlope && !isJumping)
        {
            if (debugLogDrag) { Debug.Log("Drag :: Ground"); }

            var v = playerRigidbody.velocity;
            v.y = 0f;
            v = -v * v.magnitude;

            playerRigidbody.AddForce(groundDrag * dragMultiplier * v, ForceMode.Force);
        }
        else if (isGrounded && OnSlope(transform.position).canClimpSlope && !isJumping)
        {
            if (debugLogDrag) { Debug.Log("Drag :: Slope"); }

            var v = playerRigidbody.velocity;
            v = -v * v.magnitude;

            playerRigidbody.AddForce(slopeDrag * dragMultiplier * v, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            if (debugLogDrag) { Debug.Log("Drag :: Air"); }

            var v = playerRigidbody.velocity;
            v.y = 0f;
            v = -v * v.magnitude;

            playerRigidbody.AddForce(airDrag * dragMultiplier * v, ForceMode.Force);
        }
    }

    bool isMovingControlled()
    {
        return horizontalMovement != 0 || verticalMovement != 0;
    }

    Vector3 PlayerMovingInDirection()
    {
        if (debugLogInput) { Debug.Log("vertical: " + verticalMovement + " -  horizontal: " + horizontalMovement); }
        if (debugLogPlayerDirection) { Debug.Log(orientation.forward * verticalMovement + orientation.right * horizontalMovement); }

        return orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void Jump()
    {
        if (!isGrounded) { return; }

        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
        StartCoroutine(ResetJump());
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitUntil(() => isGrounded);
        isJumping = false;
    }

    void WallJump()
    {
        if (debugLogCanWallJump) { Debug.Log("Can wall jump: " + canWallJump()); }

        if (!canWallJump() || isGrounded) { return; }

        if (Input.GetKeyDown(jumpKey))
        {
            wallJumpsInitiated++;
            playerRigidbody.velocity = Vector3.zero;
            Vector3 wallJumpDirection = (wallJumpHit.normal + Vector3.up).normalized;
            playerRigidbody.AddForce(wallJumpDirection * wallJumpForce, ForceMode.Impulse);
            StartCoroutine(ResetWallJump());
        }
    }

    IEnumerator ResetWallJump()
    {
        yield return new WaitUntil(() => isGrounded);
        wallJumpsInitiated = 0;
    }

    private Floor OnSlope(Vector3 raycastOrigin)
    {
        Floor floor = new Floor();

        if (Physics.Raycast(raycastOrigin, Vector3.down, out slopeHit, playerCollider.height / 2 + groundDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle < maxSlopeAngle && angle != 0)
            {
                // if is on a slope that player can climb
                floor.isOnSlope = true;
                floor.canClimpSlope = true;
            }
            else if (angle >= maxSlopeAngle)
            {
                // if is on a slope that player can't climb
                floor.isOnSlope = true;
                floor.canClimpSlope = false;
            }
            else if (angle == 0)
            {
                // if is on a flat surface
                floor.isOnSlope = false;
                floor.canClimpSlope = false;
            }
        }

        if (debugLogSlope) { Debug.Log("Is on slope: " + floor.isOnSlope + " Can climb slope: " + floor.canClimpSlope); }

        return floor;
    }

    private bool canWallJump()
    {
        Vector3[] directions =
        {
            orientation.forward,
            -orientation.forward,
            orientation.right,
            -orientation.right,
            orientation.forward + orientation.right,
            orientation.forward - orientation.right,
            -orientation.forward + orientation.right,
            -orientation.forward - orientation.right
        };

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(transform.position, direction, out wallJumpHit, playerCollider.radius + 0.1f) && wallJumpsInitiated < maximumWallJumps)
            {
                return true;
            }
        }
        
        return false;
    }

    private void ControlAnimation()
    {
        if (playerRigidbody.velocity.magnitude < 1f)
        {
            // set to idle
            playermodelAnimator.SetFloat("speed", Mathf.Lerp(playermodelAnimator.GetFloat("speed"), 0, 0.05f));
        }
        else if (playerRigidbody.velocity.magnitude > 0.1f && playerRigidbody.velocity.magnitude <= runAnimationTreshold)
        {
            // set to walk
            playermodelAnimator.SetFloat("speed", Mathf.Lerp(playermodelAnimator.GetFloat("speed"), 0.5f, 0.05f));
        }
        else if (playerRigidbody.velocity.magnitude > runAnimationTreshold)
        {
            // set to sprint
            playermodelAnimator.SetFloat("speed", Mathf.Lerp(playermodelAnimator.GetFloat("speed"), 1f, 0.05f));
        }
    }

    private void HandleSlope()
    {
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void StepClimb()
    {
        if (OnSlope(transform.position).isOnSlope)
        {
            return;
        }

        Vector3 pos = PlayerMovingInDirection();

        stepRayLower.position = new Vector3(playerRigidbody.position.x + pos.x, stepRayLower.position.y, playerRigidbody.position.z + pos.z);
        stepRayLower.LookAt(new Vector3(playerRigidbody.position.x + pos.x, stepRayLower.position.y - 1, playerRigidbody.position.z + pos.z));

        if (OnSlope(stepRayLower.position).isOnSlope)
        {
            return;
        }

        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.position, transform.TransformDirection(-Vector3.up), out hitLower, 2f))
        {
            if (isGrounded && (stepRayLower.position.y - hitLower.point.y <= stepHeight))
            {
                Vector3 targetVector = new Vector3(playerRigidbody.position.x, hitLower.point.y + playerCollider.height / 2, playerRigidbody.position.z) + pos * stepBoost;
                playerRigidbody.position = Vector3.Lerp(playerRigidbody.position, targetVector, Time.deltaTime / 0.1f);
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                playerRigidbody.useGravity = false;
                isStepping = true;
            }
            else if (!isGrounded && (stepRayLower.position.y - hitLower.point.y <= stepHeight))
            {
                playerRigidbody.position = new Vector3(playerRigidbody.position.x, hitLower.point.y + playerCollider.height / 2, playerRigidbody.position.z) + pos * stepBoost;
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                playerRigidbody.useGravity = false;
                isStepping = true;
            }
            else
            {
                playerRigidbody.useGravity = true;
                isStepping = false;
            }

            if (debugLogStep) { Debug.Log("Step height: " + (stepRayLower.position.y - hitLower.point.y)); }
        }
        else
        {
            playerRigidbody.useGravity = true;
            isStepping = false;
        }
    }

    class Floor { public bool isOnSlope; public bool canClimpSlope; }
}