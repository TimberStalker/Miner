using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEditor;

namespace Assets.Scripts.Input
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class DashInteraction : IInputInteraction
    {
        public float firstTapTime = 0.2f;
        public float tapDelay = 0.5f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() { }

        static DashInteraction()
        {
            InputSystem.RegisterInteraction<DashInteraction>();
        }

        void IInputInteraction.Process(ref InputInteractionContext context)
        {
            if (context.timerHasExpired)
            {
                context.Canceled();
                return;
            }

            switch (context.phase)
            {
                case InputActionPhase.Waiting:
                    if (context.ControlIsActuated(firstTapTime))
                    {
                        context.Started();
                        context.SetTimeout(tapDelay);
                    }
                    break;

                case InputActionPhase.Started:
                    if (context.ControlIsActuated())
                        context.Performed();
                    break;
            }
        }

        void IInputInteraction.Reset() { }
    }
}
