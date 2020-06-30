using UnityEngine;

namespace CelesteGameFeel.Player.States
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController controller) : base(controller)
        {

        }


        public override PlayerState HandleInput()
        {
            return null;
        }
    }
}