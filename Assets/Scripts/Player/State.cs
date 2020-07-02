using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class State : MonoBehaviour
    {
        protected Controller controller;
            
        protected State actualState;

        public State(Controller controller)
        {
            this.controller = controller;
        }

        public virtual void BeginState()
        {
        }

        public virtual void HandleInput()
        {
        }
    }
}