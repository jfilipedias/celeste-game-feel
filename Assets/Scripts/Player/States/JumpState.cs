using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class JumpState : State
    {
        private ParticleController particleController;

        public JumpState(Controller controller) : base(controller)
        {
            particleController = controller.GetComponentInChildren<ParticleController>();
        }

        #region Base Methods
        public override void Start()
        {
            base.Start();
            particleController.PlayGroundParticles();

            controller.PlayerRigidbody.velocity = Vector2.zero;
            controller.CanJumpOnWall = false;
            Jump();
        }

        public override void Update()
        {
            base.Update();

            if (elapsedTime >= controller.JumpTime)
                canChangeState = true;

            // Corner CorrectionState
            if ((controller.HitLeftCorner || controller.HitRightCorner) && !controller.HitHead)
                controller.SetState(new CornerCorrectionState(controller));
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalDirection = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (horizontalDirection == 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new StandState(controller));

            // Walk State
            if (horizontalDirection != 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new WalkState(controller));

            // Wall Slide State
            if (controller.IsOnWall && horizontalDirection == controller.FacingDirection && canChangeState)
                controller.SetState(new WallSlideState(controller));
            
            // Wall Jump State
            if (controller.IsOnWall && Input.GetButtonDown("Jump"))
                controller.SetState(new WallJumpState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall && canChangeState)
                controller.SetState(new ClimbState(controller));

            // Dash State
            if (Input.GetButtonDown("Dash") && controller.CanDash)
                controller.SetState(new DashState(controller));
        }
        #endregion

        private void Jump()
        {
            controller.PlayerRigidbody.velocity += Vector2.up * controller.JumpForce;
        }

        private void Move()
        {
            float velocityX = horizontalDirection * controller.MoveSpeed;
            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}