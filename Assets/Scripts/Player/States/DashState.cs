using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class DashState : State
    {
        private ParticleController particleController;
        private Vector2 dashDirection;

        public DashState(Controller controller) : base(controller)
        {
            particleController = controller.GetComponentInChildren<ParticleController>();
        }

        #region Base Methods
        public override void Start()
        {
            elapsedTime = 0;
            canChangeState = false;
            bool getFirstInput = false;

            controller.PlayerRigidbody.gravityScale = 0;
            controller.CanDash = false;

            while (!getFirstInput)
                getFirstInput = HandleFirstInput();

            Dash();
        }

        public override void Update()
        {
            base.Update();

            elapsedTime += Time.deltaTime;

            // Fall State
            if (elapsedTime >= controller.DashTime)
                controller.SetState(new FallState(controller));
        }

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
            particleController.StopDashParticles();

            if (dashDirection.y > 0)
                controller.PlayerRigidbody.velocity *= 0.5f;

            // TODO: Stop particles
        }
        #endregion

        public bool HandleFirstInput()
        {
            float directionX = Input.GetAxisRaw("Horizontal");
            float directionY = Input.GetAxisRaw("Vertical");

            if (directionX == 0 && (controller.IsOnGround || directionY == 0))
                horizontalDirection = controller.FacingDirection;
            else
                horizontalDirection = directionX;

            if (horizontalDirection != 0 && directionY != 0)
                dashDirection = new Vector2(horizontalDirection * 0.75f, directionY * 0.75f);
            else
                dashDirection = new Vector2(horizontalDirection, directionY);

            return true;
        }

        public void Dash()
        {
            controller.PlayerRigidbody.velocity = dashDirection * controller.DashSpeed;
            particleController.PlayDashParticles();

            // TODO: Shake camera
        }
    }
}