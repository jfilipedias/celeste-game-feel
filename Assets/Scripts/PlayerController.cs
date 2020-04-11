using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributies
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;

    [Header("Check Collision")]
    [SerializeField] private float gizmoSize;
    [SerializeField] private float collisionDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform playerFoot;
    [SerializeField] private Transform playerRightSide;
    [SerializeField] private Transform playerLeftSide;

    private float horizontalMovementDirection;
    private float verticalMovementDirection;

    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isClimbing = false;
    private bool isDashing = false;
    private bool isRightWall = false;
    private bool isLeftWall = false;
    
    private Rigidbody2D playerRigidbody2D;
    #endregion


    #region Engine Methods
    private void Awake()
    {
        playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        HandleInput();
    }


    private void FixedUpdate()
    {
        if (isJumping && isGrounded)
            Jump();
        else
            CheckGroundCollision();

        CheckWallCollistion();

        Move();

        if (isClimbing)
            Climb();
        else
            playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        if (isDashing)
            Dash();

        if (isGrounded && !isClimbing && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(playerHead.position, gizmoSize);
        Gizmos.DrawWireSphere(playerFoot.position, gizmoSize);
        Gizmos.DrawWireSphere(playerRightSide.position, gizmoSize);
        Gizmos.DrawWireSphere(playerLeftSide.position, gizmoSize);
    }
    #endregion


    #region Controller Methods
    private void HandleInput()
    {
        /**
         * Z to Climb
         * X to Dash 
         * C to Jump
         */
         
        // Horizontal Movement
        horizontalMovementDirection = Input.GetAxisRaw("Horizontal");

        // Vertical Movement
        verticalMovementDirection = Input.GetAxisRaw("Vertical");
       
        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
            isJumping = true;

        // Climb
        if (Input.GetButton("Hold") && (isRightWall || isLeftWall))
            isClimbing = true;
        else
            isClimbing = false;
    }

    
    private void Move()
    {
        float newVelocityX = horizontalMovementDirection * moveSpeed;

        playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }


    private void Jump()
    {
        isJumping = false;
        isGrounded = false;

        playerRigidbody2D.velocity += Vector2.up * jumpForce;
    }


    private void Climb()
    {
        float newVelocityY = verticalMovementDirection * climbSpeed;

        playerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        playerRigidbody2D.velocity = new Vector2(0, newVelocityY);
    }


    private void Dash()
    {

    }


    private void CheckGroundCollision()
    {
        Vector2 rayStart = (Vector2)playerFoot.position;

        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, collisionDistance, groundMask);
    }


    private void CheckWallCollistion()
    {
        Vector2 rightRayStart = (Vector2)playerRightSide.position;
        Vector2 leftRayStart = (Vector2)playerLeftSide.position;

        isRightWall = Physics2D.Raycast(rightRayStart, Vector2.right, collisionDistance, groundMask);
        isLeftWall = Physics2D.Raycast(leftRayStart, Vector2.left, collisionDistance, groundMask);
    }

    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }
    #endregion
}
