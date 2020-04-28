using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    #region Attributes
    // Show in Inspector
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float climbUpSpeed = 5f;
    [SerializeField] private float climbDownSpeed = 10f;
    [SerializeField] private float climbLedgeForce = 10f;
    [SerializeField] private float climbFowardDistance = 0.3f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float dashSpeed = 22f;

    [Space]
    [Header("Level")]
    [SerializeField] private Transform levelLimit;

    // Components
    private Rigidbody2D rb2D;
    private CollisionChecker collisionChecker;

    private float facingDirection = Vector2.right.x;
    private float moveDirection;
    private float climbDirection;
    private float defaultGravityScale;

    // Booleans
    private bool isOnGround;
    private bool isOnWall;
    private bool hitSpike;
    private bool handOnWall;
    private bool feetOnWall;

    private bool isJumping;
    private bool isWallJumping;
    private bool isClimbing;
    private bool isFlipped;
    #endregion 

    #region Proterties
    public float FacingDirection { get => facingDirection; }
    public bool IsFlipped { get => isFlipped; }
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
        
        if (hitSpike || this.transform.position.y <= levelLimit.position.y)
            Die();

        if ((moveDirection != 0 && moveDirection != facingDirection) && !isClimbing)
            FlipDirection();
    }

    private void FixedUpdate()
    {
        if (!isClimbing)
            Move();

        if (isJumping)
            Jump(Vector2.up);

        if (isWallJumping)
            WallJump();

        if (!isJumping && !isClimbing && isOnWall && moveDirection == facingDirection)
            WallSlide();

        if (isClimbing && !isJumping)
            ClimbWall();
        else
            rb2D.gravityScale = defaultGravityScale;
    }
    #endregion

    #region Controller Methods
    private void HandleInput()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");

        climbDirection = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetButtonDown("Jump") && isOnGround)
            isJumping = true;

        // Wall Jump
        if (Input.GetButton("Jump") && !isOnGround && isOnWall)
            isWallJumping = true;

        // Climb
        if (Input.GetButton("Hold") && isOnWall)
            isClimbing = true;
        else
            isClimbing = false;
    }

    private void Move()
    {
        float newVelocityX = moveDirection * moveSpeed;

        rb2D.velocity = new Vector2(newVelocityX, rb2D.velocity.y);
    }

    public void Jump(Vector2 jumpDirection)
    {
        rb2D.velocity += jumpDirection * jumpForce;

        isJumping = false;
    }

    public void WallJump()
    {
        isWallJumping = false;

        Vector2 jumpDirection = Vector2.up;

        if (moveDirection != facingDirection && moveDirection != 0)
        {
            FlipDirection();
            jumpDirection += Vector2.right * moveDirection;
        }

        Jump(jumpDirection);
    }

    public void WallSlide()
    {
        rb2D.velocity = new Vector2(0, -wallSlideSpeed);
    }

    public void ClimbWall()
    {
        rb2D.gravityScale = 0;

        if (climbDirection > 0)
            rb2D.velocity = new Vector2(0, climbUpSpeed);
        else if(climbDirection < 0)
            rb2D.velocity = new Vector2(0, -climbDownSpeed);
        else
            rb2D.velocity = Vector2.zero;
    }

    private void ClimbLedge()
    {

    }

    public void Dash()
    {

    }

    private IEnumerator WaitDashTime()
    {
        yield return null;
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
