using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WallSlideState : State
    {
        private float horizontalMovement;

        public WallSlideState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            controller.PlayerRigidbody.gravityScale = 0;
        }

        public override void FixedUpdate()
        {
            Slide();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            bool isClimbing = Input.GetButton("Hold");

            // Stand State
            if (controller.IsOnGround && !isClimbing)
                controller.SetState(new StandState(controller));

            // Climb State
            if (isClimbing)
                controller.SetState(new ClimbState(controller));

            // Fall State
            if (!controller.IsOnGround || (controller.FacingDirection != horizontalMovement))
                controller.SetState(new FallState(controller));
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