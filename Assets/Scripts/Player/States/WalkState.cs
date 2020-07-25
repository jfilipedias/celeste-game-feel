using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WalkState : State
    {
        private ParticleController particleController;
     
        public WalkState(Controller controller) : base(controller)
        {
            particleController = controller.GetComponentInChildren<ParticleController>();
        }

        #region Base Methods
        public override void Start()
        {
            if (controller.PreviousState.GetType() != typeof(WalkState) && controller.PreviousState.GetType() != typeof(ClimbState))
                particleController.PlayGroundParticles();
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalDirection = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (horizontalDirection == 0)
                controller.SetState(new StandState(controller));

            // Jump State
            if (Input.GetButtonDown("Jump") || (Input.GetButton("Jump") && controller.JumpBufferingCounter>= 0))
                controller.SetState(new JumpState(controller));

            // Wall Jump State
            if (controller.IsOnWall && Input.GetButtonDown("Jump"))
                controller.SetState(new WallJumpState(controller));

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