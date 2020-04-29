﻿using System.Collections;
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

    [Space]
    [Header("Particles")]
    [SerializeField] private ParticleSystem dashTrailParticles;
    [SerializeField] private ParticleSystem dashSpreadParticles;
    [SerializeField] private ParticleSystem groundDustParticles;

    [Space]
    [Header("Level")]
    [SerializeField] private Transform levelLimit;

    // Components
    private Rigidbody2D rb2D;
    private CollisionChecker collisionChecker;

    private float facingDirection = Vector2.right.x;
    
    private float moveDirectionX;
    private float moveDirectionY;
    
    private float defaultGravityScale;

    // Booleans
    private bool isOnGround;
    private bool isOnWall;
    private bool hitSpike;
    private bool handOnWall;
    private bool feetOnWall;

    private bool isJumping;
    private bool isWallJumping;
    private bool isClimbingWall;
    private bool isClimbingLedge;
    private bool isDashing;
    private bool isFlipped;

    private bool wasUnground = false;

    private bool canMove = true;
    private bool canClimb = true;
    private bool canWallJump = true;
    private bool canDash = true;
    #endregion 

    #region Proterties
    public float FacingDirection { get => facingDirection; }
    public bool IsFlipped { get => isFlipped; }
    public bool CanDash { get => canDash; }
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
        collisionChecker = this.GetComponent<CollisionChecker>();

        defaultGravityScale = rb2D.gravityScale;
    }

    private void Update()
    {
        CheckCollisions();

        HandleInput();

        if (wasUnground && isOnGround)
            Land();
        
        if (hitSpike || this.transform.position.y <= levelLimit.position.y)
            Die();

        if ((moveDirectionX != 0 && moveDirectionX != facingDirection) && !isClimbingWall && !isWallJumping)
            FlipDirection();

        if (isClimbingWall && !handOnWall)
            isClimbingLedge = true;

        if (!isOnGround)
            wasUnground = true;
    }

    private void FixedUpdate()
    {
        if (canMove && !isClimbingWall)
            Move();

        if (isJumping)
            Jump(Vector2.up);

        if (canWallJump && isWallJumping)
            WallJump();

        if (isOnWall && moveDirectionX == facingDirection && !isJumping && !isClimbingWall)
            WallSlide();
        else
            rb2D.gravityScale = defaultGravityScale;

        if (canClimb && isClimbingWall)
            ClimbWall();
        else
            rb2D.gravityScale = defaultGravityScale;

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

        if (Input.GetButtonDown("Jump") && isOnGround && !isClimbingWall)
            isJumping = true;

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
        if (isOnGround)
            groundDustParticles.Play();

        rb2D.velocity += jumpDirection * jumpForce;

        isJumping = false;
    }

    public void WallJump()
    {
        StartCoroutine(WaitWallJump());

        Vector2 jumpDirection = Vector2.up;

        // If not going against wall
        if (moveDirectionX != facingDirection && moveDirectionX != 0)
            jumpDirection += Vector2.right * moveDirectionX;

        rb2D.gravityScale = defaultGravityScale;

        Jump(jumpDirection);
    }

    private IEnumerator WaitWallJump()
    {
        isClimbingWall = false;
        canClimb = false;
        canWallJump = false;

        yield return new WaitForSeconds(wallJumpTime);

        isWallJumping = false;
        canClimb = true;
        canWallJump = true;
    }

    public void WallSlide()
    {
        rb2D.gravityScale = 0;
        rb2D.velocity = new Vector2(0, -wallSlideSpeed);
    }

    public void ClimbWall()
    {
        rb2D.gravityScale = 0;

        if (moveDirectionY > 0)
            rb2D.velocity = new Vector2(0, climbUpSpeed);
        else if(moveDirectionY < 0)
            rb2D.velocity = new Vector2(0, -climbDownSpeed);
        else
            rb2D.velocity = Vector2.zero;
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
        isDashing = false;
        float horizontalDirection;

        // Down dash on ground
        if (moveDirectionX == 0 && (isOnGround || moveDirectionY == 0))
            horizontalDirection = facingDirection;
        else
            horizontalDirection = moveDirectionX;

        Vector2 direction = new Vector2(horizontalDirection, moveDirectionY);

        if (direction.y >= 0)
            rb2D.gravityScale = 0;

        StartCoroutine(WaitDash());
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());

        rb2D.velocity = direction * dashSpeed;
    }

    public IEnumerator WaitDash()
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

        rb2D.velocity *= 0.8f;
        rb2D.gravityScale = defaultGravityScale;

        canMove = true;
        canClimb = true;
        canWallJump = true;
        canDash = true;
    }

    private void Land()
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

    private void CheckCollisions()
    {
        isOnGround = collisionChecker.GroundCollision();
        hitSpike = collisionChecker.SpikeCollision();
        isOnWall = collisionChecker.WallCollistion(facingDirection);
        handOnWall = collisionChecker.HandsOnWall(facingDirection);
        feetOnWall = collisionChecker.FeetOnWall(facingDirection);
    }
    #endregion
}
