using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    public Rigidbody rb;
    public Transform orientation;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float acceleration = 8f;
    private float speedMultiplier = 2f;
    private float currentSpeed;
    private Vector3 moveDirection;
    private Vector3 previousPosition;
    private float horizontalMovement;
    private float verticalMovement;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    private bool isGrounded;

    [Header("Jump & WallJump")]
    public float jumpForce = 8f;
    public float wallJumpForce = 18f;
    public int maxWallJumps = 3;
    public float fallMultiplier = 2.5f;
    private bool isJumping;
    private int wallJumpsLeft;
    private RaycastHit wallJumpHit;

    [Header("Drag")] // Add this section Claude!
    public float groundDrag = 6f;
    public float airDrag = 4f;
    public float decelerateDrag = 30.0f;
    private float dragMultiplier = 0.2f;

    #region Callbacks for Movement Abilities
    public delegate void OnPlayerJump(bool isGroundedBeforeJump);
    public OnPlayerJump onPlayerJump;

    public delegate void OnPlayerLand();
    public OnPlayerLand onPlayerLand;

    public delegate void OnPlayerSpacebar();
    public OnPlayerSpacebar onPlayerSpacebar;
    #endregion

    public void Initialize(Player playerRef)
    {
        player = playerRef;
        rb = GetComponent<Rigidbody>();
    }

    private void MyInput()
    {
        if (UIManager.IsPaused)
        {
            return;
        }

        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, acceleration * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            onPlayerSpacebar?.Invoke();
        }
    }

    public void UpdateMovement()
    {
        MyInput();

        ControlSpeed();
        MovePlayer();
    }

    public void FixedUpdateMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleDrag();
    }
    
    private void MovePlayer()
    {
        moveDirection = PlayerMovingInDirection();

        rb.AddForce(moveDirection.normalized * currentSpeed * speedMultiplier, ForceMode.Acceleration);
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, acceleration);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, acceleration);
        }
    }

    private void HandleDrag()
    {
        if (!IsMovingControlled())
        {
            var v = rb.velocity;
            v.y = 0;
            v = -v * v.magnitude;
            
            rb.AddForce(v * decelerateDrag * dragMultiplier, ForceMode.Force);

            return;
        }

        if (isGrounded)
        {
            var v = rb.velocity;
            v.y = 0;
            v = -v * v.magnitude;
            
            rb.AddForce(v * groundDrag * dragMultiplier, ForceMode.Force);
        }
        else if (!isGrounded)
        {   
            var v = rb.velocity;
            v.y = 0;
            v = -v * v.magnitude;

            rb.AddForce(v * airDrag * dragMultiplier, ForceMode.Force);
        }
    }

    private void Jump()
    {
        onPlayerJump?.Invoke(isGrounded);

        Debug.Log(isGrounded);

        if (!isGrounded)
        {
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;

        StartCoroutine(ResetJump());
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.25f); // Hardcoded for jump lagback
        yield return new WaitUntil(() => isGrounded);
        isJumping = false;

        onPlayerLand?.Invoke();
    }

    private Vector3 PlayerMovingInDirection()
    {
        return orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private bool IsMovingControlled()
    {
        return horizontalMovement != 0 || verticalMovement != 0;
    }

    #region Ability Public Methods

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        rb.AddForce(force, forceMode);
    }

    #endregion
}