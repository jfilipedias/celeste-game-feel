using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Attributies
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [Header("Check Collision")]
    [SerializeField] private float gizmoSize;
    [SerializeField] private float collisionDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform playerFoot;
    [SerializeField] private Transform playerRightSide;
    [SerializeField] private Transform playerLeftSide;

    private float movementDirection;

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

        Move();

        if (isGrounded && playerRigidbody2D.velocity.y != 0)
            ResetVerticalVelocity();
        
        //Debug.Log(playerRigidbody2D.velocity.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
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
        // Horizontal movement
        movementDirection = Input.GetAxisRaw("Horizontal");
       
        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
            isJumping = true;
    }

    
    private void Move()
    {
        float newVelocityX = movementDirection * speed;

        playerRigidbody2D.velocity =  new Vector2(newVelocityX, playerRigidbody2D.velocity.y);
    }


    private void Jump()
    {
        isJumping = false;
        isGrounded = false;

        playerRigidbody2D.velocity += Vector2.up * jumpForce;
    }


    private void CheckGroundCollision()
    {
        Vector2 rayStart = (Vector2)playerFoot.position;

        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, collisionDistance, groundMask);
    }


    private void ResetVerticalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, 0);
    }
    #endregion
}
