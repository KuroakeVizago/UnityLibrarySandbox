using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vizago.AI
{
    using Pathfinding;
    
    [RequireComponent(typeof(AILerp))]
    public class AILerpAgent : AStarMovementBase
    {
        private AILerp _aiLerpMovement;

        protected override void Awake()
        {
            base.Awake();
            
            if (_aiLerpMovement == null)
                _aiLerpMovement = GetComponent<AILerp>();
            
        }

        protected void Update()
        {

            Vector3 gravity = Vector3.zero;
            gravity.y -= m_gravityForce * Time.fixedDeltaTime;
            
            charController.Move(gravity);
            
            
        }
    }
}
