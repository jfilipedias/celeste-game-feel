using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WalkState : State
    {
        public WalkState(Controller controller) : base(controller)
        {

        }

        public void FixedUpdate()
        {
            Walk();
        }

        public override void HandleInput()
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
                controller.CurrentState = new StandState(controller);

            if (Input.GetButton("Jump"))
                controller.CurrentState = new JumpState(controller);
        }

        private void Walk()
        {
            // TODO: Create properties to get some fields values
            //controller.rb2D.velocity = new Vector2(newVelocityX, rb2D.velocity.y);
        }
    }
}