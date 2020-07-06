using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WalkState : State
    {
        private float horizontalMovement;

        public WalkState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (horizontalMovement == 0)
                controller.SetState(new StandState(controller));

            // Jump State
            if (Input.GetButtonDown("Jump"))
                controller.SetState(new JumpState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall)
                controller.SetState(new ClimbState(controller));
        }
        #endregion

        private void Move()
        {
            float velocityX = horizontalMovement * controller.MoveSpeed; 
            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}