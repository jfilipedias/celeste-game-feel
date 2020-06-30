using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class PlayerState : MonoBehaviour
    {
        protected PlayerController controller;

        public PlayerState(PlayerController controller)
        {
            this.controller = controller;
        }

        public abstract PlayerState HandleInput();
    }
}