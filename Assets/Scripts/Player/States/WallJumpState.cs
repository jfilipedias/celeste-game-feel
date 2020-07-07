using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WallJumpState : State
    {
        private float horizontalDirection;
        private float jumpHorizontalDirection;

        private float elapsedTime;
        private bool canChangeState;

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
            horizontalDirection = Input.GetAxisRaw("Horizontal");
            
            // Stand State
            if (horizontalDirection == 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new StandState(controller));

            // Walk State
            if (horizontalDirection != 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new WalkState(controller));

            // Wall Slide State
            if (controller.IsOnWall && horizontalDirection == controller.FacingDirection && canChangeState)
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
                jumpDirection = new Vector2(horizontalDirection, 1);

            controller.PlayerRigidbody.velocity += jumpDirection * controller.JumpForce;
        }

        private void Move()
        {
            float velocityX = horizontalDirection * controller.MoveSpeed;
           
            controller.PlayerRigidbody.velocity = Vector2.Lerp(controller.PlayerRigidbody.velocity, new Vector2(velocityX, controller.PlayerRigidbody.velocity.y), 5 * Time.deltaTime);
        }
    }
}