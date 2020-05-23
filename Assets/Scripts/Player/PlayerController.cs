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
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float climbUpSpeed = 5f;
    [SerializeField] private float climbDownSpeed = 10f;
    [SerializeField] private float climbLedgeForce = 8f;
    [SerializeField] private int climbLedgeIterations = 12;
    [SerializeField] private float dashSpeed = 22f;

    [Header("Timers")]
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float wallJumpTime = 0.3f;
    [SerializeField] private float jumpBufferingTime = 0.15f;
    [SerializeField] private float coyoteTime = 0.15f;

    [Space]
    [Header("Particles")]
    [SerializeField] private ParticleSystem dashTrailParticles;
    [SerializeField] private ParticleSystem dashSpreadParticles;
    [SerializeField] private ParticleSystem groundDustParticles;

    // Components
    private Rigidbody2D rb2D;
    private CollisionChecker collisionChecker;

    private Vector3 levelLimit;

    private float facingDirection = Vector2.right.x;
    
    private float moveDirectionX;
    private float moveDirectionY;
    
    private float defaultGravityScale;

    private float jumpBufferingCounter;
    private float coyoteTimeCounter;

    private int callingCount;
    
    // Booleans
    private bool isOnGround;
    private bool isOnWall;
    private bool hitSpike;
    private bool hitHeadRightSide;
    private bool hitHeadLeftSide;
    private bool hitHeadCenter;
    private bool handsOnWall;
    private bool feetOnWall;

    private bool isJumping;
    private bool isWallJumping;
    private bool isClimbingWall;
    private bool isClimbingLedge;
    private bool isDashing;
    private bool isFlipped;
    private bool isCoyoteTime;

    private bool wasUnground = false;

    private bool canMove = true;
    private bool canJump = true;
    private bool canClimb = true;
    private bool canWallJump = true;
    private bool canSlideOnWall = true;
    private bool canDash = true;
    private bool canFlip = true;
    #endregion 


    #region Proterties
    public float FacingDirection { get => facingDirection; }
    public bool IsFlipped { get => isFlipped; }
    public bool CanDash { get => canDash; }
    public bool IsOnGround { set => isOnGround = value; }
    public bool IsOnWall { set => isOnWall = value; }
    public bool HitSpike { set => hitSpike = value; }
    public bool HandOnwall { set => handsOnWall = value; }
    public bool FeetOnWall { set => feetOnWall = value; }
    #endregion


    #region MonoBehaviour Methods
    private void Awake()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
        collisionChecker = this.GetComponent<CollisionChecker>();

        levelLimit = GameObject.FindGameObjectWithTag("Level Limit").transform.position;

        defaultGravityScale = rb2D.gravityScale;
    }

    private void Update()
    {
        CheckCollisions();

        HandleInput();

        if (isOnGround)
            coyoteTimeCounter = coyoteTime;

        if (!isOnGround)
            wasUnground = true;

        if (wasUnground && isOnGround)
            Land();
        
        if (hitSpike || this.transform.position.y <= levelLimit.y)
            Die();

        if (canFlip && (moveDirectionX != 0 && moveDirectionX != facingDirection))
            FlipDirection();

        if (isClimbingWall && !handsOnWall)
            isClimbingLedge = true;

        jumpBufferingCounter -= Time.deltaTime;
        coyoteTimeCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (((hitHeadLeftSide || hitHeadRightSide) && !hitHeadCenter) && !isClimbingWall && !isOnWall)
            StartCoroutine(CornerCorrection());

        if (canMove && !isClimbingWall)
            Move();

        if (canJump && isJumping)
            Jump(Vector2.up);

        if (canWallJump && isWallJumping)
            WallJump();

        if (canSlideOnWall && (isOnWall && moveDirectionX == facingDirection) && !isJumping)
            WallSlide();
        else if(!isDashing)
            rb2D.gravityScale = defaultGravityScale;

        if (canClimb && isClimbingWall)
            ClimbWall();
        else
            StopClimbWall();

        if (isClimbingLedge)
            StartCoroutine(ClimbLedge());

        if (canDash && isDashing)
            Dash();
    }
    #endregion


    #region Controller Methods
    private void HandleInput()
    {
        moveDirectionX = Input.GetAxisRaw("Horizontal");

        moveDirectionY = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
            jumpBufferingCounter = jumpBufferingTime;

        if (Input.GetButton("Jump") && (isOnGround && !isClimbingWall && jumpBufferingCounter > 0))    // Jump Buffering
            isJumping = true;
        else if (Input.GetButton("Jump") && (!isOnGround && !isClimbingWall && coyoteTimeCounter > 0))      // CoyoteTime
        {
            isJumping = true;
            isCoyoteTime = true;
        }
        else
            isJumping = false;

        if (Input.GetButtonDown("Jump") && isOnWall && (!isOnGround || isClimbingWall))
            isWallJumping = true;

        if (Input.GetButton("Hold") && isOnWall && !isJumping && !isWallJumping)
            isClimbingWall = true;
        else
            isClimbingWall = false;

        if (Input.GetButtonDown("Dash"))
            isDashing = true;
    }

    private void Move()
    {
        float newVelocityX = moveDirectionX * moveSpeed;

        if (isWallJumping)
            rb2D.velocity = Vector2.Lerp(rb2D.velocity, new Vector2(newVelocityX, rb2D.velocity.y), 5 * Time.deltaTime);
        else
            rb2D.velocity = new Vector2(newVelocityX, rb2D.velocity.y);
    }

    public void Jump(Vector2 jumpDirection)
    {
        callingCount++;
        if (isOnGround || isCoyoteTime)
            groundDustParticles.Play();

        isCoyoteTime = false;

        rb2D.velocity += jumpDirection * jumpForce;
        
        StartCoroutine(WaitJump());
    }

    private IEnumerator WaitJump()
    {
        canJump = false;

        yield return new WaitForSeconds(coyoteTime);

        canJump = true;
    }

    public void WallJump()
    {
        canFlip = false;

        StartCoroutine(WaitWallJump());

        Vector2 jumpDirection = Vector2.up;

        // If going out from wall
        if (moveDirectionX != facingDirection && moveDirectionX != 0)
        {
            FlipDirection();
            jumpDirection += Vector2.right * moveDirectionX;
        }

        rb2D.gravityScale = defaultGravityScale;

        Jump(jumpDirection);

        canFlip = true;
    }

    private IEnumerator WaitWallJump()
    {
        isClimbingWall = false;
        canClimb = false;
        canWallJump = false;
        canSlideOnWall = false;

         yield return new WaitForSeconds(wallJumpTime);

        isWallJumping = false;
        canClimb = true;
        canWallJump = true;
        canSlideOnWall = true;
    }

    public void WallSlide()
    {
        rb2D.gravityScale = 0;
        rb2D.velocity = new Vector2(0, -wallSlideSpeed);
    }

    public void ClimbWall()
    {
        canFlip = false;
        canSlideOnWall = false;
        rb2D.gravityScale = 0;

        if (moveDirectionY > 0)
            rb2D.velocity = new Vector2(0, climbUpSpeed);
        else if(moveDirectionY < 0)
            rb2D.velocity = new Vector2(0, -climbDownSpeed);
        else
            rb2D.velocity = Vector2.zero;
    }

    private void StopClimbWall()
    {
        canFlip = true;
        canSlideOnWall = true;
        
        if(!isDashing)
            rb2D.gravityScale = defaultGravityScale;
    }

    private IEnumerator ClimbLedge()
    {
        rb2D.gravityScale = defaultGravityScale;

        int iterationCount = 0;

        while (feetOnWall)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, climbLedgeForce);
            yield return null;
        }

        while (iterationCount <= climbLedgeIterations)
        {
            rb2D.velocity = new Vector2(facingDirection * climbLedgeForce, rb2D.velocity.y);
            iterationCount++;
            yield return null;
        }

        isClimbingLedge = false;
    }

    public void Dash()
    {
        float horizontalDirection;

        // Down dash on ground
        if (moveDirectionX == 0 && (isOnGround || moveDirectionY == 0))
            horizontalDirection = facingDirection;
        else
            horizontalDirection = moveDirectionX;

        Vector2 direction = new Vector2(horizontalDirection, moveDirectionY);

        if (direction.y >= 0)
            rb2D.gravityScale = 0;

        StartCoroutine(WaitDash(direction.y)) ;
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());

        rb2D.velocity = direction * dashSpeed;
    }

    public IEnumerator WaitDash(float directionY)
    {
        canMove = false;
        canClimb = false;
        canWallJump = false;
        canDash = false;

        dashTrailParticles.Play();
        dashSpreadParticles.Play();

        yield return new WaitForSeconds(dashTime);

        dashTrailParticles.Stop();
        dashSpreadParticles.Stop();

        if (directionY > 0)
            rb2D.velocity *= 0.5f;
        else
            rb2D.velocity *= 0.8f;
        
        rb2D.gravityScale = defaultGravityScale;

        isDashing = false;
        canMove = true;
        canClimb = true;
        canWallJump = true;
    }

    public IEnumerator CornerCorrection()
    {
        Debug.Log("Start corner correction");
        rb2D.bodyType = RigidbodyType2D.Kinematic;

        float direction = 1f;

        if (hitHeadRightSide)
            direction = -1f;

        while (hitHeadLeftSide || hitHeadRightSide)
        {
            Debug.Log("Loop through");

            Vector2 targetPosition = new Vector2(direction, jumpForce) * Time.deltaTime;
           
            rb2D.MovePosition(rb2D.position + targetPosition);

            CheckHeadCollision();

            yield return null;
        }

        if(!(hitHeadLeftSide && hitHeadRightSide))
        {
            rb2D.bodyType = RigidbodyType2D.Dynamic;
            //rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);

            Debug.Log("Stop correction");
        }

    }

    private void Land()
    {
        canDash = true;
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

    private void CheckCollisions()
    {
        isOnGround = collisionChecker.GroundCollision();
        hitSpike = collisionChecker.SpikeCollision();
        isOnWall = collisionChecker.WallCollision();
        handsOnWall = collisionChecker.HandsOnWall();
        feetOnWall = collisionChecker.FeetOnWall();

        CheckHeadCollision();
    }

    private void CheckHeadCollision()
    {
        hitHeadRightSide = collisionChecker.RightHeadSideCollision();
        hitHeadLeftSide = collisionChecker.LeftHeadSideCollision();
        hitHeadCenter = collisionChecker.HeadCenterCollision();
    }
    #endregion
}
