using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributies
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbLedgeForce;
    [SerializeField] private float climbFowardDistance;
    [SerializeField] private float dashForce;

    private Rigidbody2D playerRigidbody2D;
    private SpriteRenderer playerSprite;
    private PlayerCollision playerCollision;

    private float horizontalMovementDirection;
    private float verticalMovementDirection;

    private float facingDirection = Vector2.right.x;

    private bool isOnGround = false;
    private bool isOnWall = false;
    private bool isHandOnWall = false;
    private bool isFeetOnWall = false;
    private bool isJumping = false;
    private bool isClimbing = false;
    private bool isDashing = false;
    #endregion

    #region Properties
    public float FacingDirection { get => facingDirection; }
    #endregion

    #region Engine Methods
    private void Awake()
    {
        playerRigidbody2D = this.GetComponent<Rigidbody2D>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        playerCollision = this.GetComponent<PlayerCollision>();
    }

    private void Update()
    {
        HandleInput();

        if (facingDirection != horizontalMovementDirection && horizontalMovementDirection != 0 && !isClimbing)
            Flip();
    }

    private void FixedUpdate()
    {
        // Check Colision
        CheckCollision();

        // Move
        if(!isClimbing)
            Move();

        // Jump
        if (isJumping && isOnGround)
            Jump();

        // Climb        
        if (isClimbing)
            Climb();

        if (isClimbing && !isHandOnWall)
            StartCoroutine(ClimbLedge());

        // Wall Slide
        if (isOnWall && !isClimbing && !isOnGround && horizontalMovementDirection == facingDirection)
            WallSlide();

        // Dash
        if (isDashing)
            Dash();

        // Reset Y Velocity
        if (isOnGround && !isClimbing && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();

        // Control Rigidbody Mode
        if(!isClimbing 
            && !(isOnWall && !isOnGround && horizontalMovementDirection == facingDirection))
            SetDinamicRigidbody2D();
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

        SetKinematicRigidbody2D(true);

        float newVelocityY = verticalMovementDirection * climbSpeed;

        playerRigidbody2D.velocity = new Vector2(0, newVelocityY);
    }

    private IEnumerator ClimbLedge()
    {
        SetDinamicRigidbody2D();

        while (isFeetOnWall)
        {
            playerRigidbody2D.velocity = new Vector2(0, climbLedgeForce);

            yield return null;
        }

        Vector2 foward = playerRigidbody2D.position + new Vector2(climbFowardDistance * facingDirection, 0);

        playerRigidbody2D.MovePosition(foward);

        yield return new WaitForFixedUpdate();
    }

    private void WallSlide()
    {
        bool freezePositionX = true;
        SetKinematicRigidbody2D(freezePositionX);

        playerRigidbody2D.velocity = new Vector2(0, wallSlideSpeed) * Vector2.down;
    }

    private void Dash()
    {

    }

    private void Flip()
    {
        facingDirection *= -1;
        playerSprite.flipX = !playerSprite.flipX;
    }

    private void CheckCollision()
    {
        if(!isJumping)
            isOnGround = playerCollision.CheckGroundCollision();
        
        isOnWall = playerCollision.CheckWallCollistion(facingDirection);
        isHandOnWall = playerCollision.CheckHandsOnWall(facingDirection);
        isFeetOnWall = playerCollision.CheckFeetOnWall(facingDirection);
    }

    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }

    private void SetDinamicRigidbody2D()
    {
        playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    } 

    private void SetKinematicRigidbody2D(bool freezePositionX)
    {
        playerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        
        if (freezePositionX)
            playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }
    #endregion
}
