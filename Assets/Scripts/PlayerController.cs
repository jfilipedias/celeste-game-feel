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
    [SerializeField] private float jumpOverWallForce;
    [SerializeField] private float dashForce;

    [Header("Check Collision")]
    [SerializeField] private float groundCollisionDistance;
    [SerializeField] private float wallCollisionDistance;
    [SerializeField] private float sphereGizmoSize;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform playerKnee;
    [SerializeField] private Transform playerRightFoot;
    [SerializeField] private Transform playerLeftFoot;

    private Rigidbody2D playerRigidbody2D;
    private SpriteRenderer playerSprite;

    private float horizontalMovementDirection;
    private float verticalMovementDirection;

    private Vector2 facingDirection = Vector2.right;

    private bool isOnGround = false;
    private bool isOnWall = false;
    private bool isFacingWall = false;
    private bool isJumping = false;
    private bool isClimbing = false;
    private bool isDashing = false;
    #endregion


    #region Engine Methods
    private void Awake()
    {
        playerRigidbody2D = this.GetComponent<Rigidbody2D>();
        playerSprite = this.GetComponent<SpriteRenderer>();
    }


    private void Update()
    {
        HandleInput();

        if (facingDirection.x != horizontalMovementDirection && horizontalMovementDirection != 0)
            Flip();
    }


    private void FixedUpdate()
    {
        // Check Colision
        if(!isOnGround)
            CheckGroundCollision();
    
        CheckWallCollistion();

        // Move
        if (!isClimbing)
            Move();

        // Jump
        if (isJumping && isOnGround)
            Jump();

        //Climb        
        if (isClimbing)
            Climb();
        else
            playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        if (isClimbing && !isFacingWall)
            JumpOverWall();

        // Dash
        if (isDashing)
            Dash();

        // Reset Y Velocity
        if (isOnGround && !isClimbing && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        // Face
        Gizmos.DrawWireSphere(playerKnee.position, sphereGizmoSize);
        Gizmos.DrawLine(playerKnee.position, (playerKnee.position + new Vector3(wallCollisionDistance * facingDirection.x, 0, 0)));
        
        // Hand
        Gizmos.DrawWireSphere(playerHand.position, sphereGizmoSize);
        Gizmos.DrawLine(playerHand.position, (playerHand.position + new Vector3(wallCollisionDistance * facingDirection.x, 0, 0)));
        
        // Foot
        Gizmos.DrawWireSphere(playerRightFoot.position, sphereGizmoSize);
        Gizmos.DrawWireSphere(playerLeftFoot.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerRightFoot.position, (playerRightFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
        Gizmos.DrawLine(playerLeftFoot.position, (playerLeftFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
    }
    #endregion


    #region Controller Methods
    private void HandleInput()
    {         
        // Horizontal Movement
        horizontalMovementDirection = Input.GetAxisRaw("Horizontal");

        // Vertical Movement
        verticalMovementDirection = Input.GetAxisRaw("Vertical");
       
        // Jumping
        if (Input.GetButtonDown("Jump") && isOnGround)
            isJumping = true;

        // Climb
        if (Input.GetButton("Hold") && isOnWall)
            isClimbing = true;
        else
            isClimbing = false;
    }

    
    private void Move()
    {
        Debug.Log("Move");
        float newVelocityX = horizontalMovementDirection * moveSpeed;

        playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }


    private void Jump()
    {
        playerRigidbody2D.velocity += Vector2.up * jumpForce;
        
        isJumping = false;
        isOnGround = false;
    }


    private void Climb()
    {
        isOnGround = false;

        float newVelocityY = verticalMovementDirection * climbSpeed;

        playerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        playerRigidbody2D.velocity = new Vector2(0, newVelocityY);
    }


    private void JumpOverWall()
    {

    }

    private void Dash()
    {

    }


    private void Flip()
    {
        facingDirection.x *= -1;
        playerSprite.flipX = !playerSprite.flipX;
    }


    private void CheckGroundCollision()
    {
        Vector2 rightRayStart = (Vector2)playerRightFoot.position;
        Vector2 leftRayStart = (Vector2)playerLeftFoot.position;

        bool rightFootOnGround = Physics2D.Raycast(rightRayStart, Vector2.down, groundCollisionDistance, groundMask);
        bool leftFootOnGround = Physics2D.Raycast(leftRayStart, Vector2.down, groundCollisionDistance, groundMask);

        isOnGround = rightFootOnGround || leftFootOnGround ? true : false;

    }


    private void CheckWallCollistion()
    {
        Vector2 handRayStart = (Vector2)playerHand.position;
        isOnWall = Physics2D.Raycast(handRayStart, facingDirection, wallCollisionDistance, groundMask);

        Vector2 faceRayStart = (Vector2)playerKnee.position;
        isFacingWall = Physics2D.Raycast(faceRayStart, facingDirection, wallCollisionDistance, groundMask);
    }


    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }
    #endregion
}
