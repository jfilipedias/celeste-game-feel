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
    [SerializeField] private float groundCollisionDistance;
    [SerializeField] private float wallCollisionDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform playerFace;
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform playerFoot;

    private Rigidbody2D playerRigidbody2D;
    private SpriteRenderer playerSprite;

    private float horizontalMovementDirection;
    private float verticalMovementDirection;

    private Vector2 facingDirection = Vector2.right;

    private bool isGrounded = false;
    private bool isWall = false;
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
        if (isJumping && isGrounded)
            Jump();
        else
            CheckGroundCollision();

        CheckWallCollistion();

        Run();

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

        // Face
        Gizmos.DrawWireSphere(playerFace.position, gizmoSize);
        Gizmos.DrawLine(playerFace.position, (playerFace.position + new Vector3(wallCollisionDistance * facingDirection.x, 0, 0)));
        
        // Hand
        Gizmos.DrawWireSphere(playerHand.position, gizmoSize);
        Gizmos.DrawLine(playerHand.position, (playerHand.position + new Vector3(wallCollisionDistance * facingDirection.x, 0, 0)));
        
        // Foot
        Gizmos.DrawWireSphere(playerFoot.position, gizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerFoot.position, (playerFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
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
        if (Input.GetButtonDown("Jump") && isGrounded)
            isJumping = true;

        // Climb
        if (Input.GetButton("Hold") && isWall)
            isClimbing = true;
        else
            isClimbing = false;
    }

    
    private void Run()
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


    private void Flip()
    {
        facingDirection.x *= -1;
        playerSprite.flipX = !playerSprite.flipX;
    }


    private void CheckGroundCollision()
    {
        Vector2 rayStart = (Vector2)playerFoot.position;
        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, groundCollisionDistance, groundMask);
    }


    private void CheckWallCollistion()
    {
        Vector2 rayStart = (Vector2)playerHand.position;
        isWall = Physics2D.Raycast(rayStart, facingDirection, wallCollisionDistance, groundMask);
    }


    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }
    #endregion
}
