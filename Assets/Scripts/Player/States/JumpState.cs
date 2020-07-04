using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace CelesteGameFeel.Player.States
{
    public class JumpState : State
    {
        private float horizontalMovement;

        public JumpState(Controller controller) : base(controller)
        {
        }

        public override void Start()
        {
            controller.PlayerRigidbody.velocity = Vector2.zero;

            Jump();
        }

        public override void FixedUpdate()
        {
            Move();
        }

        public override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            if (Input.GetAxisRaw("Horizontal") == 0 && controller.IsOnGround)
                controller.SetState(new StandState(controller));

            if (Input.GetAxisRaw("Horizontal") != 0 && controller.IsOnGround)
                controller.SetState(new WalkState(controller));
        }

        private void Jump()
        {
            controller.PlayerRigidbody.velocity += Vector2.up * controller.JumpForce;
        }

        private void Move()
        {
            float velocityX = horizontalMovement * 10;
            controller.PlayerRigidbody.velocity = new Vector2(velocityX, controller.PlayerRigidbody.velocity.y);
        }
    }
}