using CelesteGameFeel.Player;
using UnityEngine;

namespace Assets.Scripts.Player.States
{
    public class DashState : State
    {
        private float horizontalDirection;
        private float verticalDirection;

        private Vector2 dashDirection;

        private float elapsedTime;
        private bool canChangeState;

        public DashState(Controller controller) : base(controller)
        {
            elapsedTime = 0;
            canChangeState = false;
        }

        #region Base Methods
        public override void Start()
        {
            bool getFirstInput = false;

            controller.PlayerRigidbody.gravityScale = 0;

            while(getFirstInput)
                getFirstInput = HandleFirstInput();

            Dash();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Finish()
        {
            controller.PlayerRigidbody.gravityScale = controller.DefaultGravityScale;
            
            if (dashDirection.y > 0)
                controller.PlayerRigidbody.velocity *= 0.5f;
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
        
            // TODO: Shake camera
            // TODO: Play particles
        }
    }
}