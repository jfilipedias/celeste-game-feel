using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class JumpState : State
    {
        private float horizontalMovement;

        public JumpState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            controller.PlayerRigidbody.velocity = Vector2.zero;

            Jump();
        }

        public override void Update()
        {
            base.Update();

            // Wall Slide State
            if (controller.IsOnWall && horizontalMovement == controller.FacingDirection)
                controller.SetState(new WallSlideState(controller));
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (Input.GetAxisRaw("Horizontal") == 0 && controller.IsOnGround)
                controller.SetState(new StandState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall)
                controller.SetState(new ClimbState(controller));

            if (Input.GetAxisRaw("Horizontal") != 0 && controller.IsOnGround)
                controller.SetState(new WalkState(controller));
        }
        #endregion

        private void Jump()
        {
            controller.PlayerRigidbody.velocity += Vector2.up * controller.JumpForce;
        }

        private void Move()
        {
            float velocityX = horizontalMovement * controller.MoveSpeed;
            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}