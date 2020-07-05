using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class StandState : State
    {
        public StandState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            controller.PlayerRigidbody.velocity = Vector2.zero;
        }

        protected override void HandleInput()
        {
            // Jump State
            if (Input.GetButtonDown("Jump"))
                controller.SetState(new JumpState(controller));

            // Walk State
            if (Input.GetAxisRaw("Horizontal") != 0)
                controller.SetState(new WalkState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall)
                controller.SetState(new ClimbState(controller));
        }
        #endregion
    }
}