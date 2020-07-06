using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class JumpState : State
    {
        private float horizontalMovement;
        private float elapsedTime;
        private bool canChangeState;

        public JumpState(Controller controller) : base(controller)
        {
            elapsedTime = 0;
            canChangeState = false;
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

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= controller.WaitJump)
                canChangeState = true;
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            // Stand State
            if (horizontalMovement == 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new StandState(controller));

            // Walk State
            if (horizontalMovement != 0 && controller.IsOnGround && canChangeState)
                controller.SetState(new WalkState(controller));

            // Wall Slide State
            if (controller.IsOnWall && horizontalMovement == controller.FacingDirection && canChangeState)
                controller.SetState(new WallSlideState(controller));

            // Climb State
            if (Input.GetButton("Hold") && controller.IsOnWall && canChangeState)
                controller.SetState(new ClimbState(controller));
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