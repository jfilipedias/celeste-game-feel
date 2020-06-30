using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class IdleState : State
    {
        public IdleState(Controller controller) : base(controller)
        {

        }

        public override State HandleInput()
        {
            return null;
        }
    }
}