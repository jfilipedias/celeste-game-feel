using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class FallState : State
    {
        private float horizontalDirection;

        public FallState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalDirection = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (controller.IsOnGround && !(Input.GetButton("Hold") && controller.IsOnWall))
                controller.SetState(new StandState(controller));

            // Wall Slide State
            if (controller.IsOnWall && horizontalDirection == controller.FacingDirection)
                controller.SetState(new WallSlideState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall)
                controller.SetState(new ClimbState(controller));

            // Dash State
            if (Input.GetButtonDown("Dash") && controller.CanDash)
                controller.SetState(new DashState(controller));
        }
        #endregion

        private void Move()
        {
            float velocityX = horizontalDirection * controller.MoveSpeed;
            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}