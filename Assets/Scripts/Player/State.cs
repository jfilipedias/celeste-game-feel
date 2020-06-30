using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class State : MonoBehaviour
    {
        protected Controller controller;

        public State(Controller controller)
        {
            this.controller = controller;
        }

        public abstract State HandleInput();
    }
}