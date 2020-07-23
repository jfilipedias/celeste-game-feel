using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class CornerCorrectionState : State
    {
        private float direction;
        public CornerCorrectionState(Controller controller) : base(controller)
        {
        }

        #region Base Methods
        public override void Start()
        {
            controller.BoxCollider.enabled = false; 
            
            controller.PlayerRigidbody.velocity = new Vector2(controller.PlayerRigidbody.velocity.x, controller.StoredVelocityY);

            direction = 1f;

            if (controller.HitRightCorner)
                direction = -1f;
        }

        public override void FixedUpdate()
        {
            if (controller.HitLeftCorner || controller.HitRightCorner)
                Rectify();
            else
                controller.SetState(new FallState(controller));
        }

        public override void Finish()
        {
            controller.PlayerRigidbody.velocity = new Vector2(0, controller.PlayerRigidbody.velocity.y);
            controller.BoxCollider.enabled = true;
        }
        #endregion
    
        private void Rectify()
        {
            controller.PlayerRigidbody.velocity = new Vector2(direction, controller.PlayerRigidbody.velocity.y);

            controller.CheckHeadCollisions();

            Debug.Log("Looping throw");
        }
    }
}