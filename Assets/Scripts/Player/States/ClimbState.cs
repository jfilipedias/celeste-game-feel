using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class ClimbState : State
    {
        public ClimbState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            base.Start();

            controller.PlayerRigidbody.gravityScale = 0;
            controller.CanFlipDirection = false;
        }

        public override void Update()
        {
            base.Update();

            // TODO: Fix climb if isn't in wall
            // TODO: Implement ledge climb
        }

        public override void FixedUpdate()
        {
            Climb();
        }

        protected override void HandleInput()
        {
            horizontalDirection = Input.GetAxisRaw("Horizontal");
            verticalDirection = Input.GetAxisRaw("Vertical");

            bool isClimbing = Input.GetButton("Hold");

            // Stand State
            if (!isClimbing && controller.IsOnGround)
                controller.SetState(new StandState(controller));

            // Fall State
            if (!isClimbing && !controller.IsOnGround)
                controller.SetState(new FallState (controller));

            // Wall Jump State
            if (Input.GetButtonDown("Jump"))
                controller.SetState(new WallJumpState(controller));

            // Wall Slide State
            if (!isClimbing && horizontalDirection == controller.FacingDirection)
                controller.SetState(new WallSlideState(controller));

            // Dash State
            if (Input.GetButtonDown("Dash") && controller.CanDash)
                controller.SetState(new DashState(controller));

        }

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
            controller.CanFlipDirection = true;
        }
        #endregion

        private void Climb()
        {
            float velocityY = verticalDirection * controller.ClimbSpeed;

            if (verticalDirection > 0)
                controller.PlayerRigidbody.velocity = new Vector2(0, controller.ClimbSpeed);
            else if (verticalDirection < 0)
                controller.PlayerRigidbody.velocity = new Vector2(0, -controller.ClimbSpeed * 2);
            else
                controller.PlayerRigidbody.velocity = Vector2.zero;
        }
    }
}