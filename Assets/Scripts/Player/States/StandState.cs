using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class StandState : State
    {
        public StandState(Controller controller) : base(controller)
        {
        }

        public override void HandleInput()
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
                controller.CurrentState = new WalkState(controller);

            if (Input.GetButton("Jump"))
                controller.CurrentState = new JumpState(controller);
        }
    }
}