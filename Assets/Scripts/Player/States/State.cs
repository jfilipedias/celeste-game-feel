using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class State
    {
        protected Controller controller;

        // TODO: Put some common variables

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

        protected virtual void HandleInput()
        {
        }

        public virtual void Finish()
        {
        }
    }
}