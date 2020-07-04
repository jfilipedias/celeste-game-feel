using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class StandState : State
    {
        public StandState(Controller controller) : base(controller)
        {
        }

        public override void Start()
        {
            controller.PlayerRigidbody.velocity = Vector2.zero;
        }

        public override void HandleInput()
        {
            if (Input.GetButton("Jump"))
                controller.SetState(new JumpState(controller));
         
            if (Input.GetAxisRaw("Horizontal") != 0)
                controller.SetState(new WalkState(controller));
        }
    }
}