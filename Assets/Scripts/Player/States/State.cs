using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class State
    {
        protected Controller controller;
            
        protected State actualState;

        public State(Controller controller)
        {
            this.controller = controller;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
            HandleInput();
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void HandleInput()
        {
        }
    }
}