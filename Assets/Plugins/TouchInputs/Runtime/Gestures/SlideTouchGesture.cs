using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualIT.TouchInputs
{
    public class SlideTouchGesture : BaseTouchGesture
    {
        [Header("Slide properties")]
        [Tooltip("Minimum delta distance that can be done to be triggered")]
        public float MinMouvementDistance = 50;
        [Space(10)]
        public UnityEvent<SlideTouchData> OnGestureStarted;
        public UnityEvent<SlideTouchData> OnGesturePerformed;
        public UnityEvent<SlideTouchData> OnGestureCanceled;

        private SlideTouchData slideData = new SlideTouchData();
        private bool isGestureStarted = false;
        private bool isGestureBehaviourStarted = false;
        private Vector2 delta = Vector2.zero;
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
            else if (TestTouchRestriction(touches.Length) && !isGestureStarted && isATouchBeginningThisFrame(touches)) // Start tracking
            {
                // Start gesture
                isGestureStarted = true;
                startTime = Time.time;
                slideData = new SlideTouchData();
                delta = Vector2.zero;
            }


            if (isGestureStarted) // If touches are tracked by Gesture
            {
                slideData.FrameDeltaPosition = GetAverageDelaPosition(touches);
                slideData.CurrentPosition = GetAveragePosition(touches);
                delta += slideData.FrameDeltaPosition;

                if (!isGestureBehaviourStarted && Time.time - startTime >= MinDuration && delta.magnitude >= MinMouvementDistance) // Start gesture condition
                {
                    slideData.StartPosition = GetAveragePosition(touches);
                    isGestureBehaviourStarted = true;
                    OnGestureStarted.Invoke(slideData);
                }
                if (isGestureBehaviourStarted) // Update gesture
                {
                    if (slideData.FrameDeltaPosition != Vector2.zero)
                    {
                        slideData.GlobalDeltaPosition += slideData.FrameDeltaPosition;
                        OnGesturePerformed.Invoke(slideData);
                    }

                    if (Time.time - startTime > MaxDuration)
                    {
                        // Cancel because max gesture duration not respected
                        ResetGesture(touches);
                    }
                }
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
            slideData = new SlideTouchData();
        }



        private SlideTouchData simulatedSlideData = new SlideTouchData();
        private bool simulationStarted = false;
        private bool simulationBehaviourStarted = false;
        private Vector2 simulatedDelta = Vector2.zero;
        private float simulatedStartTime;

        private int simulatedTouchCount = 0;
        private Vector2 simulatedTouchPosition;
        private Vector2 simulatedTouchDelta;
        private int simulatedOldTouchCount = 0;
        internal override void UpdateGestureSimulation()
        {
            if (simulatedThisFrame && TestTouchRestriction(simulatedTouchCount))
            {
                if (!simulatedLastFrame && simulatedOldTouchCount < simulatedTouchCount) // Start simulation 
                {
                    if (!simulationStarted)
                    {
                        simulationStarted = true;
                        simulatedStartTime = Time.time;
                        simulatedDelta = Vector2.zero;
                    }
                }

                // Simulation
                if (simulationStarted)
                {
                    simulatedSlideData.FrameDeltaPosition = simulatedTouchDelta;
                    simulatedSlideData.CurrentPosition = simulatedTouchPosition;
                    simulatedDelta += simulatedSlideData.FrameDeltaPosition;

                    if (!simulationBehaviourStarted && Time.time - simulatedStartTime >= MinDuration && simulatedDelta.magnitude >= MinMouvementDistance)
                    {
                        simulatedSlideData.StartPosition = simulatedTouchPosition;
                        OnGestureStarted.Invoke(simulatedSlideData);
                        simulationBehaviourStarted = true;
                    }
                    if (simulationBehaviourStarted)
                    {
                        if (simulatedSlideData.FrameDeltaPosition != Vector2.zero)
                        {
                            simulatedSlideData.GlobalDeltaPosition += simulatedSlideData.FrameDeltaPosition;
                            OnGesturePerformed.Invoke(simulatedSlideData);
                        }

                        if (Time.time - simulatedStartTime > MaxDuration)
                        {
                            ResetSimulation();
                        }
                    }
                }

                simulatedThisFrame = false;
                simulatedLastFrame = true;
            }
            else if (simulatedLastFrame) // End simulation
            {
                ResetSimulation();
                simulatedLastFrame = false;
                simulatedTouchCount = 0;
            }
            else
            {
                simulatedTouchCount = 0;
            }
            simulatedOldTouchCount = simulatedTouchCount;
        }

        public void SimulateSlide(int nbTouch, Vector2 position, Vector2 deltaPosition)
        {
            simulatedThisFrame = true;

            simulatedTouchCount = nbTouch;
            simulatedTouchPosition = position;
            simulatedTouchDelta = deltaPosition;
        }
        private void ResetSimulation()
        {
            if (simulationBehaviourStarted)
            {
                OnGestureCanceled.Invoke(simulatedSlideData);
            }
            simulationStarted = false;
            simulationBehaviourStarted = false;
            simulatedDelta = Vector2.zero;
            simulatedStartTime = 0;
            simulatedSlideData = new SlideTouchData();
        }

        public class SlideTouchData
        {
            public Vector2 StartPosition;
            public Vector2 CurrentPosition;
            public Vector2 FrameDeltaPosition;
            public Vector2 GlobalDeltaPosition;
        }

#if UNITY_EDITOR
        [MenuItem("Virtual-IT/TouchInputs/Create Slide gesture...", priority = 1)]
        internal static void CreateScriptableObject()
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            string directory = (string)args[0];

            SlideTouchGesture newLogger = CreateInstance<SlideTouchGesture>();
            AssetDatabase.CreateAsset(newLogger, AssetDatabase.GenerateUniqueAssetPath(directory + "/SlideTouceGesture.asset"));
            Selection.activeObject = newLogger;
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
