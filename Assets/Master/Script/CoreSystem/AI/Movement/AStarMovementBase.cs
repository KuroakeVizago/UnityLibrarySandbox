using System;
using NaughtyAttributes;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

namespace Vizago.AI
{
    [RequireComponent(typeof(Seeker), typeof(SimpleSmoothModifier), typeof(CharacterController))]
    public abstract class AStarMovementBase : MonoBehaviour
    {

        [Header("Config:")] 
        [SerializeField] protected float m_gravityForce = 0.98f;

        [SerializeField] protected UnityEvent m_onReachDestination;
        // TODO: [SerializeField] protected UnityEvent m_onInterrupted;

        public UnityAction OnReachDestination { get; set; }
        // TODO: public UnityAction OnInterrupted { get; set; }
        
        protected IAstarAI astarAI;
        protected CharacterController charController;
        protected Vector3 currentDestination;

        private bool _destinationReached;
        
        #region Editor Function
        
        protected virtual void Reset()
        {
            if (astarAI != null)
            {
                DestroyImmediate(astarAI as MonoBehaviour);
            }
        }

        //[Button("Use Lerp AI Movement", EButtonEnableMode.Editor)]
        //private void AddLerpMovement()
        //{
        //    if (astarAI is AILerp) return;
        //    if (astarAI != null)
        //        DestroyImmediate(astarAI as MonoBehaviour);
        //        
        //    astarAI = gameObject.AddComponent<AILerp>();
        //}
        //
        //[Button("Use Path AI Movement", EButtonEnableMode.Editor)]
        //private void AddPathAIMovement()
        //{
        //    if (astarAI is AIPath) return;
        //    if (astarAI != null)
        //        DestroyImmediate(astarAI as MonoBehaviour);
        //        
        //    astarAI = gameObject.AddComponent<AIPath>();
        //}

        #endregion
        
        protected virtual void Awake()
        {
            if (astarAI == null)
                astarAI = GetComponent<IAstarAI>();
            else
                throw new NullReferenceException(nameof(astarAI));
            
            if (charController == null)
                charController = GetComponent<CharacterController>();
            else
                throw new NullReferenceException(nameof(charController));

        }

        protected virtual void Start()
        {
            astarAI.canMove = false;
            charController.detectCollisions = true;
        }

        protected virtual void Update()
        {
            if (astarAI.reachedEndOfPath)
            {
                if (!_destinationReached)
                {
                    ForceStop();
                    
                    _destinationReached = true;
                    OnReachDestination?.Invoke();
                    m_onReachDestination?.Invoke();
                }
            }
            else
            {
                _destinationReached = false;
            }

        }
        
        public virtual void MoveTo(Transform target)
        {
            if (!target) return;
            astarAI.destination = target.position;
            astarAI.canMove = true;
        }

        public virtual void MoveTo(Vector3 target)
        {
            
            astarAI.destination = target;
            astarAI.canMove = true;

        }

        public virtual void ForceStop()
        {
            if (astarAI.position != astarAI.destination && astarAI.canMove)
            {
                astarAI.canMove = false;
                astarAI.destination = Vector3.zero;
            }
        }
        
    }
}
