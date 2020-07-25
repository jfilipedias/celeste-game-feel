using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class StandState : State
    {
        private ParticleController particleController;

        public StandState(Controller controller) : base(controller)
        {
            particleController = controller.GetComponentInChildren<ParticleController>();
        }

        #region Base Methods
        public override void Start()
        {
            controller.PlayerRigidbody.velocity = Vector2.zero;
         
            if (controller.PreviousState.GetType() != typeof(WalkState) && controller.PreviousState.GetType() != typeof(ClimbState))
                particleController.PlayGroundParticles();
        }

        protected override void HandleInput()
        {
            // Jump State
            if (Input.GetButtonDown("Jump") || (Input.GetButton("Jump") && controller.JumpBufferingCounter >= 0))
                controller.SetState(new JumpState(controller));

            // Walk State
            if (Input.GetAxisRaw("Horizontal") != 0)
                controller.SetState(new WalkState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall)
                controller.SetState(new ClimbState(controller));

            // Dash State
            if (Input.GetButtonDown("Dash") && controller.CanDash)
                controller.SetState(new DashState(controller));
        }
        #endregion
    }
}