using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributes
    // Show in Inspector
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float climbDownSpeed = 10f;
    [SerializeField] private float climbLedgeForce = 10f;
    [SerializeField] private float climbFowardDistance = 0.3f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float dashSpeed = 22f;

    [Space]
    [Header("Timers")]
    [SerializeField] private float disabledMoveTime = 0.3f;
    [SerializeField] private float disabledJumpTime = 0.3f;
    [SerializeField] private float disabledClimbTime = 0.3f;
    [SerializeField] private float disabledWallSlideTime = 0.3f;
    [SerializeField] private float dashWaitTime = 0.15f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferingtime = 0.15f;

    [Space]
    [Header("Particles")]
    [SerializeField] private ParticleSystem dashTrailParticles;
    [SerializeField] private ParticleSystem dashSpreadParticles;
    [SerializeField] private ParticleSystem groundDustParticles;

    [Space]
    [Header("Level")]
    [SerializeField] private Transform levelLimit;

    // Game Object Components
    private Rigidbody2D playerRigidbody2D;
    private PlayerCollision playerCollision;

    // Default Values
    private float defaultGravityScale;

    private float jumpBufferingElapsed;

    // Movement
    private float horizontalMovementDirection;
    private float verticalMovementDirection;
    private float facingDirection = Vector2.right.x;

    // Booleans
    private bool onGround = false;
    private bool onWall = false;
    private bool onSpike = false;
    
    private bool handOnWall = false;
    private bool feetOnWall = false;
    
    private bool isJumping = false;
    private bool isWallJumping = false;
    private bool isClimbing = false;
    private bool isDashing = false;
    private bool isFlipped = false;
    private bool isCoyoteTime = false;
    private bool isJumpBuffering = false;

    private bool wasWallJumping = false;
    private bool wasUnground = false;
    
    private bool canMove = true;
    private bool canJump = true;
    private bool canClimb = true;
    private bool canDash = true;
    private bool canSlideOnWall = true;
    private bool canCoyoteTime = true;
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

        defaultGravityScale = playerRigidbody2D.gravityScale;
    }

    private void Update()
    {
        HandleInput();

        ControlBooleans();

        if (onSpike || this.transform.position.y <= levelLimit.position.y)
            Die();

        if ((horizontalMovementDirection != facingDirection && horizontalMovementDirection != 0) && !isClimbing && !isWallJumping && (canMove || wasWallJumping))
            FlipDirection();

        if (isDashing)
            StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());

        if (wasUnground && onGround)
            Landing();

        if (isCoyoteTime && !isJumping)
            StartCoroutine(WaitCoyoteTime());
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        if(canMove && !isClimbing && !isWallJumping)
            Move();

        if ((isJumping || isJumpBuffering) && (onGround || isCoyoteTime) && !isClimbing)
            Jump(Vector2.up);

        if (onWall && isWallJumping)
            WallJump();

        if (onWall && canSlideOnWall && !isClimbing && !onGround && horizontalMovementDirection == facingDirection)
            WallSlide();

        if (isClimbing && !isWallJumping)
            Climb();
        else
            SetGravityScaleValue(defaultGravityScale);

        if (isClimbing && !handOnWall)
            StartCoroutine(ClimbLedge());

        if (isDashing)
            Dash();

        if (onGround && !isClimbing && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();
    }
    #endregion

    #region Controller Methods
    private void HandleInput()
    {         
        // Horizontal Movement
        horizontalMovementDirection = Input.GetAxisRaw("Horizontal");

        // Vertical Movement
        verticalMovementDirection = Input.GetAxisRaw("Vertical");
       
        // Jump
        if (Input.GetButtonDown("Jump") && canJump && (onGround || isCoyoteTime))
            isJumping = true;

        // Jump Buffering
        ControllJumpBuffering();

        // Wall Jump
        if (Input.GetButtonDown("Jump") && onWall && !onGround && canJump)
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
        
        // Prevent move against wall
        if (!(onWall && horizontalMovementDirection == facingDirection))
            playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }

    private void Jump(Vector2 jumpDirection)
    {
        canCoyoteTime = false;

        if (onGround || isCoyoteTime)
            groundDustParticles.Play();

        playerRigidbody2D.velocity += jumpDirection * jumpForce;

        onGround = false;
        isJumping = false;
    }

    private void ControllJumpBuffering()
    {
        jumpBufferingElapsed -= Time.deltaTime;

        if (Input.GetButton("Jump") && !isJumpBuffering)
            jumpBufferingElapsed = jumpBufferingtime;

        if (jumpBufferingElapsed > 0)
            isJumpBuffering = true;
        else
            isJumpBuffering = false;
    }

    private void WallJump()
    {
        isWallJumping = false;

        StartCoroutine(DisableClimb());
        StartCoroutine(DisableJump());
        StartCoroutine(DisableWallSlide());
        
        SetGravityScaleValue(defaultGravityScale);

        Vector2 horizontalDirection = Vector2.zero;

        if (horizontalMovementDirection != facingDirection && horizontalMovementDirection != 0)
        {
            StartCoroutine(DisableMovement());
            FlipDirection();
            horizontalDirection = Vector2.right * horizontalMovementDirection;
        }

        Vector2 jumpDirection = Vector2.up + (horizontalDirection / 2);

        Jump(jumpDirection);

        wasWallJumping = true;
    }

    private void WallSlide()
    {
        SetGravityScaleValue(0);

        playerRigidbody2D.velocity = new Vector2(0, wallSlideSpeed) * Vector2.down;
    }

    private void Climb()
    {
        onGround = false;

        SetGravityScaleValue(0);

        float newVelocityY = 0f;

        if (verticalMovementDirection > 0)
            newVelocityY = verticalMovementDirection * climbSpeed;
        else if (verticalMovementDirection < 0 && !onGround)
            newVelocityY = verticalMovementDirection * climbDownSpeed;

        playerRigidbody2D.velocity = new Vector2(0, newVelocityY);
    }

    private IEnumerator ClimbLedge()
    {
        SetGravityScaleValue(defaultGravityScale);

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

        dashTrailParticles.Play();
        dashSpreadParticles.Play();

        if (direction.y >= 0)
            SetGravityScaleValue(0);

        yield return new WaitForSeconds(dashWaitTime);

        if (direction.y >= 0)
            playerRigidbody2D.velocity *= 0.5f;
        else
            playerRigidbody2D.velocity *= 0.8f;

        SetGravityScaleValue(defaultGravityScale);

        canMove = true;
        canJump = true;
        isDashing = false;

        dashTrailParticles.Stop();
        dashSpreadParticles.Stop();
        yield return new WaitForFixedUpdate();
    }

    private void Landing()
    {
        wasUnground = false;
        groundDustParticles.Play();
    }

    private void Die()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void FlipDirection()
    {
        facingDirection *= -1;
        isFlipped = !isFlipped;
    }

    private IEnumerator WaitCoyoteTime()
    {
        canCoyoteTime = false;

        yield return new WaitForSeconds(coyoteTime);

        isCoyoteTime = false;
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

    private IEnumerator DisableWallSlide()
    {
        canSlideOnWall = false;

        yield return new WaitForSeconds(disabledWallSlideTime);

        canSlideOnWall = true;
    }

    private void CheckCollisions()
    {
        onGround = playerCollision.CheckGroundCollision();
        onSpike = playerCollision.CheckSpikeCollision();
        onWall = playerCollision.CheckWallCollistion(facingDirection);
        handOnWall = playerCollision.CheckHandsOnWall(facingDirection);
        feetOnWall = playerCollision.CheckFeetOnWall(facingDirection);
    }

    private void ControlBooleans()
    {
        if (wasWallJumping)
            wasWallJumping = false;

        if (!onGround)
            wasUnground = true;

        if (onGround && !isDashing)
            canDash = true;

        if (!onGround && !isJumping && canCoyoteTime)
            isCoyoteTime = true;

        if (onGround)
            canCoyoteTime = true;
    }

    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }

    private void SetGravityScaleValue(float newGravityScaleValue)
    {
        playerRigidbody2D.gravityScale = newGravityScaleValue;
    }
    #endregion
}
