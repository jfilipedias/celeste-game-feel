using CelesteGameFeel.Player.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CelesteGameFeel.Player
{
    public class Controller : MonoBehaviour
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
        [SerializeField] private int climbLedgeIterations = 15;
        [SerializeField] private float dashSpeed = 22f;

        // Components
        private BoxCollider2D boxCollider;
        private Rigidbody2D playerRigidbody;
        private CollisionHandler collisionHandler;

        // Player Classes
        private State currentState;
        private State previousState;

        private Vector3 levelLimit;

        private float facingDirection = Vector2.right.x;

        private float defaultGravityScale;

        private float jumpBufferingCounter;
        private float coyoteTimeCounter;
        private float storedVelocityY;
        private float dashElapsedCounter;

        private int callingCount;

        // Booleans
        private bool isOnGround;
        private bool isOnWall;
        private bool hitSpike;
        private bool hitRightCorner;
        private bool hitLeftCorner;
        private bool hitHead;
        private bool handsOnWall;
        private bool feetOnWall;

        private bool isFlipped;
        private bool isCornerCorrection;

        private bool canDash = true;
        #endregion


        #region Proterties
        public Rigidbody2D PlayerRigidbody { get => playerRigidbody; }
        public float FacingDirection { get => facingDirection; }
        public bool IsFlipped { get => isFlipped; }
        public bool CanDash { get => canDash; }
        #endregion


        #region MonoBehaviour Methods
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            playerRigidbody = GetComponent<Rigidbody2D>();
            collisionHandler = GetComponent<CollisionHandler>();

            //levelLimit = GameObject.FindGameObjectWithTag("Level Limit").transform.position;

            //defaultGravityScale = rb2D.gravityScale;

            currentState = new StandState(this);
        }

        private void Update()
        {
            currentState.HandleInput();
        }

        private void FixedUpdate()
        {
            currentState.FixedUpdate();
        }
        #endregion


        #region Controller Methods
        // TODO: Add xbox controller support
        
        // TODO: Implement stamina to wall climb

        // TODO: Fix Wall Climb after dash and press arrow left and arrow up

        // TODO: Fix dash that suddenly stop

        public void SetState(State newState)
        {
            previousState = currentState;
            currentState = newState;

            currentState.Start();
        }

        private void FlipDirection()
        {
            facingDirection *= -1;
            isFlipped = !isFlipped;
        }

        private void CheckCollisions()
        {
            isOnGround = collisionHandler.GroundCollision();

            if (!isOnGround && !isCornerCorrection)
                CheckHeadCollisions();

            hitSpike = collisionHandler.SpikeCollision();
            isOnWall = collisionHandler.WallCollision();
            handsOnWall = collisionHandler.HandsOnWall();
            feetOnWall = collisionHandler.FeetOnWall();
        }

        private void CheckHeadCollisions()
        {
            hitRightCorner = collisionHandler.RightCornerOnWall();
            hitLeftCorner = collisionHandler.LeftCornerOnWall();
            hitHead = collisionHandler.HeadOnWall();
        }
        #endregion
    }
}