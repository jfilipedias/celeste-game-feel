using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class WallJumpState : State
    {
        private float horizontalMovement;
        private float verticalMovement;

        public WallJumpState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            HandleInput();

            Jump();
        }

        public override void FixedUpdate()
        {
            Move();
        }

        protected override void HandleInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");
        }

        #endregion

        private void Jump()
        {

        }

        private void Move()
        {

        }
    }
}