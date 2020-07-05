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

        public override void Update()
        {
            base.Update();

            // Fall State
            if (controller.FacingDirection != horizontalMovement)
                controller.SetState(new FallState(controller));
        }

        public override void FixedUpdate()
        {
            Slide();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            // Climb State
            if (Input.GetButton("Hold"))
                controller.SetState(new ClimbState(controller));
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