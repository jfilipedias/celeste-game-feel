using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WalkState : State
    {
        private float walkDirection;

        public WalkState(Controller controller) : base(controller)
        {
        }

        public override void FixedUpdate()
        {
            Walk();
        }

        public override void HandleInput()
        {
            walkDirection = Input.GetAxisRaw("Horizontal");

            if (Input.GetAxisRaw("Horizontal") == 0)
                controller.SetState(new StandState(controller));

            if (Input.GetButton("Jump"))
                controller.SetState(new JumpState(controller));
        }

        private void Walk()
        {
            float velocityX = walkDirection * 10; 

            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}