using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vizago.InputManager
{
    [DefaultExecutionOrder(-1000000000)]
    public sealed class GameplayInputManager : InputManagerBase
    {

        public InputSystem.CharacterInputActions CharacterInput { get; private set; }

        public static GameplayInputManager Instance { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            CharacterInput = InputSystemInstance.CharacterInput;
            
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
            
        }

    }
}