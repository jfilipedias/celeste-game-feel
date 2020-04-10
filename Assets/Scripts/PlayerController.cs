using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributies

    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedIncreaser;
    [SerializeField] private float jumpForce;

    [Header("Check Collision")]
    [SerializeField] private Transform playerFoot;
    [SerializeField] private float gizmoSize;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;

    private float movementDirection;
    private float speed;

    private bool isJumping = false;
    private bool isGrounded = false;
    
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

        if (movementDirection != 0)
            Move();
        else
            ResetSpeed();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(playerFoot.position, gizmoSize);
    }
    #endregion


    #region Controller Methods
    private void HandleInput()
    {
        // Horizontal movement
        movementDirection = Input.GetAxisRaw("Horizontal");
       
        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
            isJumping = true;
    }

    
    private void Move()
    {
        SmoothSpeed();

        float newVelocityX = movementDirection * speed;

        playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }


    private void Jump()
    {
        Debug.Log("Jump");
        isJumping = false;
        isGrounded = false;

        playerRigidbody2D.velocity += Vector2.up * jumpForce;
    }


    private void SmoothSpeed()
    {
        if (speed < maxSpeed)
            speed *= speedIncreaser;
        else
            speed = maxSpeed;
    }


    private void ResetSpeed()
    {
        speed = 1f;
        playerRigidbody2D.velocity = new Vector2(0, playerRigidbody2D.velocity.y);
    }


    private void CheckGroundCollision()
    {
        Vector2 rayStart = (Vector2)playerFoot.position;

        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, groundDistance, groundMask);
    }
    #endregion
}
