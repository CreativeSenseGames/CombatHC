using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualIT.TouchInputs
{
    public class StretchTouchGesture : BaseTouchGesture
    {
        [Header("Stretch properties")]
        [Tooltip("Minimum delta distance that can be done to be triggered")]
        public float MinStretchDistance = 50;
        [Space(10)]
        public UnityEvent<StetchTouchData> OnGestureStarted;
        public UnityEvent<StetchTouchData> OnGesturePerformed;
        public UnityEvent<StetchTouchData> OnGestureCanceled;

        private StetchTouchData slideData = new StetchTouchData();
        private bool isGestureStarted = false;
        private bool isGestureBehaviourStarted = false;

        private float startTime;


        private bool simulatedThisFrame = false;
        private bool simulatedLastFrame = false;

        internal override void UpdateGesture(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            if (!TestTouchRestriction(touches.Length) && isGestureStarted) // End tracking
            {
                // Cancel because finger count not respected
                ResetGesture(touches);
            }
            else if (TestTouchRestriction(touches.Length) && !isGestureStarted && isATouchBeginningThisFrame(touches) && touches.Length > 1) // Start tracking && touches > 1 => impossible with only one finger
            {
                // Start gesture
                isGestureStarted = true;
                startTime = Time.time;
                slideData = new StetchTouchData();
                slideData.Stretch = 1;
                slideData.StretchCenterPosition = GetAveragePosition(touches);
            }


            if (isGestureStarted) // If touches are tracked by Gesture
            {
                slideData.StretchCenterPosition = GetAveragePosition(touches);
                float stretchFrameGlobal = 0;
                Vector2 stretchFrame = Vector2.zero;
                foreach (var touch in touches)
                {
                    Vector2 lastFramePosition = touch.screenPosition - touch.delta;

                    float relativeDistanceStart = Vector2.Distance(lastFramePosition, slideData.StretchCenterPosition);
                    float relativeDistanceEnd = Vector2.Distance(touch.screenPosition, slideData.StretchCenterPosition);
                    stretchFrameGlobal += relativeDistanceEnd / relativeDistanceStart;

                    Vector2 centerRelativePositionStart = lastFramePosition - slideData.StretchCenterPosition;
                    Vector2 centerRelativePositionEnd = touch.screenPosition - slideData.StretchCenterPosition;
                    //Vector2 relativeDelta = Vector3.Project(touch.delta, centerRelativePositionEnd);
                    stretchFrame += new Vector2(touch.delta.x / centerRelativePositionStart.x, touch.delta.y / centerRelativePositionStart.y) + Vector2.one;
                }
                slideData.FrameStretch = stretchFrameGlobal / touches.Length;
                slideData.Stretch *= slideData.FrameStretch;

                if (!isGestureBehaviourStarted && Time.time - startTime > MinDuration && Mathf.Abs(slideData.Stretch - 1) > MinStretchDistance / 100) // Start gesture condition
                {
                    isGestureBehaviourStarted = true;
                    OnGestureStarted.Invoke(slideData);
                }
                if (isGestureBehaviourStarted) // Update gesture
                {
                    OnGesturePerformed.Invoke(slideData);

                    if (Time.time - startTime > MaxDuration)
                    {
                        // Cancel because max gesture duration not respected
                        ResetGesture(touches);
                    }
                }
            }
        }
        internal override void UpdateGestureSimulation()
        {
            if (simulatedThisFrame)
            {
                if (!simulatedLastFrame) // Start simulation 
                {

                }

                // Simulation
                simulatedThisFrame = false;
                simulatedLastFrame = true;
            }
            else if (simulatedLastFrame) // End simulation
            {
                simulatedLastFrame = false;
            }
        }

        private void ResetGesture(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            if (isGestureBehaviourStarted)
            {
                OnGestureCanceled.Invoke(slideData);
            }

            isGestureStarted = false;
            isGestureBehaviourStarted = false;

            startTime = 0;
            slideData = new StetchTouchData();
            slideData.Stretch = 1;
        }

        public class StetchTouchData
        {
            public Vector2 StretchCenterPosition;
            public float Stretch;
            public float FrameStretch;
        }

        public void SimulateStretch()
        {
            simulatedThisFrame = true;
        }
    }
}
