using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class ClimbState : State
    {
        private float verticalMovement;

        public ClimbState(Controller controller) : base(controller)
        {
        }

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
            verticalMovement = Input.GetAxisRaw("Vertical");

            // Stand State
            if (!Input.GetButton("Hold") && controller.IsOnGround)
                controller.SetState(new StandState(controller));

            // Fall State
            if (!Input.GetButton("Hold") && !controller.IsOnGround)
                controller.SetState(new FallState (controller));
        }

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

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
            controller.CanFlipDirection = true;
        }
    }
}