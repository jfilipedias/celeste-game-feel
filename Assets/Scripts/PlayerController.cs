using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributes
    // Show in Inspector
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float climbLedgeForce;
    [SerializeField] private float climbFowardDistance;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float dashSpeed;

    [Space]
    [Header("Timers")]
    [SerializeField] private float disabledMoveTime;
    [SerializeField] private float disabledJumpTime;
    [SerializeField] private float disabledClimbTime;
    [SerializeField] private float dashWaitTime;

    [Space]
    [Header("Particles")]
    [SerializeField] private ParticleSystem dashParticles;

    // Game Object Components
    private Rigidbody2D playerRigidbody2D;
    private PlayerCollision playerCollision;

    private float initialGravityScale;

    // Movement
    private float horizontalMovementDirection;
    private float verticalMovementDirection;
    private float facingDirection = Vector2.right.x;

    // Booleans
    private bool onGround = false;
    private bool onWall = false;
    
    private bool handOnWall = false;
    private bool feetOnWall = false;
    
    private bool isJumping = false;
    private bool isWallJumping = false;
    private bool isClimbing = false;
    private bool isDashing = false;
    private bool isFlipped = false;
    
    private bool canMove = true;
    private bool canJump = true;
    private bool canClimb = true;
    private bool canDash = true;
    #endregion

    #region Properties
    public float FacingDirection { get => facingDirection; }
    public bool CanDash { get => canDash; }
    public bool IsFlipped { get => isFlipped; }
    #endregion

    #region Engine Methods
    private void Awake()
    {
        playerRigidbody2D = this.GetComponent<Rigidbody2D>();
        playerCollision = this.GetComponent<PlayerCollision>();

        initialGravityScale = playerRigidbody2D.gravityScale;
    }

    private void Update()
    {
        HandleInput();

        if (facingDirection != horizontalMovementDirection && horizontalMovementDirection != 0 && !isClimbing && !isWallJumping)
            FlipDirection();

        if (isDashing)
            StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());
    }

    private void FixedUpdate()
    {
        CheckCollision();

        if(canMove && !isClimbing && !isWallJumping)
            Move();

        if (isJumping && onGround && !isClimbing)
            Jump(Vector2.up);

        if (canJump && onWall && isWallJumping)
            WallJump();

        if (onWall && !isClimbing && !onGround && horizontalMovementDirection == facingDirection)
            WallSlide();

        if (isClimbing && !isWallJumping)
            Climb();
        else
            SetDinamicRigidbody2D();

        if (isClimbing && !handOnWall)
            StartCoroutine(ClimbLedge());

        if (isDashing)
            Dash();

        if (onGround && !isClimbing && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();

        if (onGround && !isDashing)
            canDash = true;
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
        if (Input.GetButtonDown("Jump") && onGround)
            isJumping = true;

        // Wall Jump
        if (Input.GetButtonDown("Jump") && onWall && !onGround)
            isWallJumping = true;

        // Climb
        if (Input.GetButton("Hold") && canClimb && onWall && !isWallJumping)
            isClimbing = true;
        else
            isClimbing = false;

        // Dash
        if (Input.GetButtonDown("Dash") && canDash)
            isDashing = true;
    }
    
    private void Move()
    {
        float newVelocityX = horizontalMovementDirection * moveSpeed;

        playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }

    private void Jump(Vector2 jumpDirection)
    {
        playerRigidbody2D.velocity += jumpDirection * jumpForce;
        
        onGround = false;
        isJumping = false;
    }

    private void WallJump()
    {
        isWallJumping = false;
        
        SetDinamicRigidbody2D();

        StartCoroutine(DisableMovement());
        StartCoroutine(DisableClimb());
        StartCoroutine(DisableJump());

        Vector2 horizontalDirection = Vector2.right * horizontalMovementDirection;

        Vector2 jumpDirection = Vector2.up + horizontalDirection / 2;

        Jump(jumpDirection);
    }

    private void WallSlide()
    {
        bool freezePositionX = true;
        SetKinematicRigidbody2D(freezePositionX);

        playerRigidbody2D.velocity = new Vector2(0, wallSlideSpeed) * Vector2.down;
    }

    private void Climb()
    {
        onGround = false;

        SetKinematicRigidbody2D(true);

        float newVelocityY = verticalMovementDirection * climbSpeed;
 
        playerRigidbody2D.velocity = new Vector2(0, newVelocityY);
    }

    private IEnumerator ClimbLedge()
    {
        SetDinamicRigidbody2D();

        while (feetOnWall)
        {
            playerRigidbody2D.velocity += Vector2.up * climbLedgeForce;

            yield return null;
        }

        ResetVerticalVelocity();

        Vector2 foward = playerRigidbody2D.position + (Vector2.right * climbFowardDistance * facingDirection);

        playerRigidbody2D.MovePosition(foward);
    }

    private void Dash()
    {
        canDash = false;

        float horizontalDirection;

        if (horizontalMovementDirection == 0 && (onGround || verticalMovementDirection == 0))
            horizontalDirection = facingDirection;
        else
            horizontalDirection = horizontalMovementDirection;

        Vector2 direction = new Vector2(horizontalDirection, verticalMovementDirection);

        StartCoroutine(WaitDashTime(direction));

        playerRigidbody2D.velocity = direction * dashSpeed;
    }

    private IEnumerator WaitDashTime(Vector2 direction)
    {
        canMove = false;
        canJump = false;
        canDash = false;

        dashParticles.Play();

        if (direction.y >= 0)
            playerRigidbody2D.gravityScale = 0f;

        yield return new WaitForSeconds(dashWaitTime);

        if (direction.y >= 0)
            playerRigidbody2D.velocity *= 0.5f;
        else
            playerRigidbody2D.velocity *= 0.8f;

        playerRigidbody2D.gravityScale = initialGravityScale;

        canMove = true;
        canJump = true;
        isDashing = false;

        dashParticles.Stop();
        yield return new WaitForFixedUpdate();
    }

    private void FlipDirection()
    {
        facingDirection *= -1;
        isFlipped = !isFlipped;
    }

    private IEnumerator DisableMovement()
    {
        canMove = false;

        yield return new WaitForSeconds(disabledMoveTime);

        canMove = true;
    }
    
    private IEnumerator DisableJump()
    {
        canJump = false;

        yield return new WaitForSeconds(disabledJumpTime);

        canJump = true;
    }

    private IEnumerator DisableClimb()
    {
        canClimb = false;

        yield return new WaitForSeconds(disabledClimbTime);

        canClimb = true;
    }

    private void CheckCollision()
    {
        if(!isJumping)
            onGround = playerCollision.CheckGroundCollision();
        
        onWall = playerCollision.CheckWallCollistion(facingDirection);
        handOnWall = playerCollision.CheckHandsOnWall(facingDirection);
        feetOnWall = playerCollision.CheckFeetOnWall(facingDirection);
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
