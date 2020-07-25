using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class ClimbLedgeState : State
    {
        private int iterationCount = 0;
        
        public ClimbLedgeState(Controller controller) : base(controller)
        {
        }

        public override void Start()
        {
            base.Start();

            Climb();
        }

        public override void Update()
        {
            base.Update();

            if (elapsedTime >= 0.3f)
                controller.SetState(new FallState(controller));
        }

        private async Task Climb()
        {

            while (controller.IsFeetOnWall)
            {
                controller.PlayerRigidbody.velocity = new Vector2(controller.PlayerRigidbody.velocity.x, controller.ClimbLedgeForce);
                await Task.Yield();
            }

            while (iterationCount <= controller.ClimbLedgeIterations)
            {
                controller.PlayerRigidbody.velocity = new Vector2(controller.FacingDirection * controller.ClimbLedgeForce, controller.PlayerRigidbody.velocity.y);
                iterationCount++;
                await Task.Yield();
            }
        }
    }
}