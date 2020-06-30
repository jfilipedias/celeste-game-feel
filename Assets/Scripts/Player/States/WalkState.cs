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

        public override State HandleInput()
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
                return new IdleState(controller);

            return null;
        }

        private void Walk()
        {
            // TODO: Create properties to get some fields values
            //controller.rb2D.velocity = new Vector2(newVelocityX, rb2D.velocity.y);
        }
    }
}