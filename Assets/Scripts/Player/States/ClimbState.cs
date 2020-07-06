    using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class ClimbState : State
    {
        private float horizontalMovement;
        private float verticalMovement;

        public ClimbState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            controller.PlayerRigidbody.gravityScale = 0;
            controller.CanFlipDirection = false;
        }

        public override void FixedUpdate()
        {
            Climb();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");

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
            if (!isClimbing && horizontalMovement == controller.FacingDirection)
                controller.SetState(new WallSlideState(controller));

        }

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
            controller.CanFlipDirection = true;
        }
        #endregion

        private void Climb()
        {
            float velocityY = verticalMovement * controller.ClimbSpeed;

            if (verticalMovement > 0)
                controller.PlayerRigidbody.velocity = new Vector2(0, controller.ClimbSpeed);
            else if (verticalMovement < 0)
                controller.PlayerRigidbody.velocity = new Vector2(0, -controller.ClimbSpeed * 2);
            else
                controller.PlayerRigidbody.velocity = Vector2.zero;
        }
    }
}