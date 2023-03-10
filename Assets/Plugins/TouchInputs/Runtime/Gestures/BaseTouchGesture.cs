using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace VirtualIT.TouchInputs
{
    public abstract class BaseTouchGesture : ScriptableObject
    {
        [Header("Generic properties")]
        [Tooltip("Maximum duration of touch to be performed")]
        public float MaxDuration = 0.2f; // If Instant or Custom
        [Tooltip("Minimum duration of touch to be performed")]
        public float MinDuration = 0.2f; // If Long or Custom
        [Tooltip("How many touch are necessary to trigger the gesture")]
        public TouchCount TouchRestriction = 0;
        [Tooltip("Enable touch simulation by other devices")]
        public bool EnableSimulation = true;

        /// <summary>
        /// Enable gesture. Specify a screen in case of multiscreen setup if you want the gesture to be enabled only on a specific screen. If not specified, gesture will get enabled on all TouchScreens.
        /// </summary>
        /// <param name="screenToApplyGestureOn"></param>
        public void EnableGesture(Touchscreen screenToApplyGestureOn = null)
        {
            TouchManager.Instance.EnableTouchGesture(this, screenToApplyGestureOn);
        }
        /// <summary>
        /// Disable gesture. Specify a screen in case of multiscreen setup if you want the gesture to be disable only on a specific screen. If not specified, gesture will be disable on all TouchScreens.
        /// </summary>
        /// <param name="screenToRemoveGestureOn"></param>
        public void DisableGesture(Touchscreen screenToRemoveGestureOn = null)
        {
            TouchManager.Instance.DisableTouchGesture(this, screenToRemoveGestureOn);
        }

        internal abstract void UpdateGesture(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches);
        internal abstract void UpdateGestureSimulation();
        protected Vector2 GetAveragePosition(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            Vector2 averagePosition = Vector2.zero;
            foreach (var touch in touches)
            {
                averagePosition += touch.screenPosition;
            }
            averagePosition /= touches.Length;
            return averagePosition;
        }
        protected Vector2 GetAverageDelaPosition(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            Vector2 averageDeltaPosition = Vector2.zero;
            foreach (var touch in touches)
            {
                averageDeltaPosition += touch.delta;
            }
            averageDeltaPosition /= touches.Length;
            return averageDeltaPosition;
        }
        protected bool isATouchBeginningThisFrame(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            foreach (var touch in touches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) return true;
            }
            return false;
        }
        protected bool isATouchEnddedThisFrame(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            foreach (var touch in touches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) return true;
            }
            return false;
        }
        protected bool TestTouchRestriction(int touchCount)
        {
            return touchCount > 10 && TouchRestriction.HasFlag(TouchCount.TenMore) || touchCount <= 10 && touchCount > 0 && TouchRestriction.HasFlag((TouchCount)(1 << touchCount));
        }
    }
}
