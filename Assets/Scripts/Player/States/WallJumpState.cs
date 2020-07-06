using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WallJumpState : State
    {
        private float horizontalMovement;
        private float jumpHorizontalDirection;
        private float elapsedTime;

        private bool canChangeState;
        private bool canFlipDirection;

        public WallJumpState(Controller controller) : base(controller)
        {
            elapsedTime = 0;
            canChangeState = false;
        }

        #region Base Methods
        public override void Start()
        {
            controller.CanFlipDirection = false;

            jumpHorizontalDirection = Input.GetAxisRaw("Horizontal");

            if (jumpHorizontalDirection == -controller.FacingDirection)
                controller.FlipDirection();
            else
                controller.CanFlipDirection = true;

            Jump();
        }

        public override void Update()
        {
            base.Update();

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= controller.WaitWallJump)
                controller.SetState(new FallState(controller));
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            
            // Stand State
            if (horizontalMovement == 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new StandState(controller));

            // Walk State
            if (horizontalMovement != 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new WalkState(controller));

            // Wall Slide State
            if (controller.IsOnWall && horizontalMovement == controller.FacingDirection && canChangeState)
                controller.SetState(new WallSlideState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall && canChangeState)
                controller.SetState(new ClimbState(controller));
        }

        public override void Finish()
        {
            controller.CanFlipDirection = true;
        }
        #endregion

        private void Jump()
        {
            Vector2 jumpDirection;

            if (jumpHorizontalDirection == controller.FacingDirection || jumpHorizontalDirection == 0)
                jumpDirection = Vector2.up;
            else
                jumpDirection = new Vector2(horizontalMovement, 1);

            controller.PlayerRigidbody.velocity += jumpDirection * controller.JumpForce;
        }

        private void Move()
        {
            float velocityX = horizontalMovement * controller.MoveSpeed;
           
            controller.PlayerRigidbody.velocity = Vector2.Lerp(controller.PlayerRigidbody.velocity, new Vector2(velocityX, controller.PlayerRigidbody.velocity.y), 5 * Time.deltaTime);
        }
    }
}