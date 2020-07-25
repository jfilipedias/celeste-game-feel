using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WallSlideState : State
    {
        public WallSlideState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            base.Start();

            controller.PlayerRigidbody.gravityScale = 0;
        }

        public override void FixedUpdate()
        {
            Slide();
        }

        protected override void HandleInput()
        {
            horizontalDirection = Input.GetAxisRaw("Horizontal");

            bool isClimbing = Input.GetButton("Hold");

            // Stand State
            if (controller.IsOnGround && !isClimbing)
                controller.SetState(new StandState(controller));

            // Wall Jump State
            if (controller.IsOnWall && Input.GetButtonDown("Jump"))
                controller.SetState(new WallJumpState(controller));

            // Climb State
            if (isClimbing)
                controller.SetState(new ClimbState(controller));

            // Fall State
            if (!controller.IsOnGround || (controller.FacingDirection != horizontalDirection))
                controller.SetState(new FallState(controller));

            // Dash State
            if (Input.GetButtonDown("Dash") && controller.CanDash)
                controller.SetState(new DashState(controller));
        }

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
        }
        #endregion

        private void Slide()
        {
            controller.PlayerRigidbody.velocity = new Vector2(0, -controller.WallSlideSpeed);
        }
    }
}