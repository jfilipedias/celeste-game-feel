using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WalkState : State
    {
        private float horizontalMovement;

        public WalkState(Controller controller) : base(controller)
        {
        }

        public override void FixedUpdate()
        {
            Move();
        }

        public override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            if (Input.GetButton("Jump"))
                controller.SetState(new JumpState(controller));

            if (Input.GetAxisRaw("Horizontal") == 0)
                controller.SetState(new StandState(controller));
        }

        private void Move()
        {
            float velocityX = horizontalMovement * 10; 

            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}