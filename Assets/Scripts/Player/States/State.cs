﻿using UnityEngine;

namespace CelesteGameFeel.Player
{
    public abstract class State
    {
        protected Controller controller;

        protected float horizontalDirection;
        protected float verticalDirection;

        protected float elapsedTime;
        protected bool canChangeState;

        public State(Controller controller)
        {
            this.controller = controller;
        } 

        public virtual void Start()
        {
            elapsedTime = 0;
            canChangeState = false;
        }

        public virtual void Update()
        {
            elapsedTime += Time.deltaTime;
         
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