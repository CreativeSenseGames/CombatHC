using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;

namespace VirtualIT.TouchInputs
{
    public class TapTouchGesture : BaseTouchGesture
    {
        [Header("Tap properties")]
        [Min(1), Tooltip("How many tap to do before triggering")]
        public int MultiTapCountToTrigger = 1;
        [Tooltip("Delay maximum between every taps. Ignored if MultiTapCount = 1")]
        public float MaxMultiTapDelay = 0.2f;
        [Tooltip("Maximum delta distance that can be done to be triggered")]
        public float MaxMouvementDistance = 50;
        [Space(10)]
        public UnityEvent<Vector2> OnGestureStarted;
        public UnityEvent<Vector2> OnGesturePerformed;
        public UnityEvent<Vector2> OnGestureCanceled;

        #region Touch Gesture
        private bool isGestureStarted = false;
        private bool touchStarted = false;
        private bool isBehaviourStarted = false;
        private float startTime;

        private Vector2 tapPosition = Vector2.zero;
        private Vector2 delta = Vector2.zero;
        private int tapCount;

        internal override void UpdateGesture(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            if (!TestTouchRestriction(touches.Length) && isGestureStarted && touchStarted) // End tracking
            {
                // Cancel because finger count not respected
                ResetGesture();
            }
            else if (TestTouchRestriction(touches.Length) && !isGestureStarted && isATouchBeginningThisFrame(touches)) // Start tracking
            {
                // Start gesture
                isGestureStarted = true;
                touchStarted = true;
                startTime = Time.time;
                delta = Vector2.zero;
                tapCount = 0;
            }
            else if (isGestureStarted && !touchStarted && isATouchBeginningThisFrame(touches) && TestTouchRestriction(touches.Length)) // ReStart tracking
            {
                touchStarted = true;
                startTime = Time.time;
            }


            if (isGestureStarted && touchStarted) // Update if behaviour started and 1 touch on screen
            {
                if (!isBehaviourStarted && Time.time - startTime > MinDuration) // Start gesture condition
                {
                    tapPosition = GetAveragePosition(touches);
                    isBehaviourStarted = true;
                    OnGestureStarted.Invoke(tapPosition);
                }

                if (isBehaviourStarted) // Update behaviour
                {
                    if (isTouchEndingThisFrame(touches))
                    {
                        if (Time.time - startTime > MinDuration)
                        {
                            // Tap 
                            tapCount++;
                            if (tapCount == MultiTapCountToTrigger)
                            {
                                OnGesturePerformed.Invoke(GetAveragePosition(touches));
                                ResetGesture();
                            }
                            else
                            {
                                // New tap wait
                                touchStarted = false;
                                delta = Vector2.zero;
                                startTime = Time.time;
                            }
                        }
                        else
                        {
                            // Cancel because min time not respected
                            ResetGesture();
                        }
                    }
                    else if (Time.time - startTime > MaxDuration)
                    {
                        // Cancel because max time not respected
                        ResetGesture();
                    }
                    else
                    {
                        // Update 
                        delta += GetAverageDelaPosition(touches); // Use average or full sum?
                        if (delta.magnitude > MaxMouvementDistance)
                        {
                            // Cancel because max delta distance not respected
                            ResetGesture();
                        }
                    }
                }
            }
            else if (isGestureStarted && !touchStarted)
            {
                if (Time.time - startTime > MaxMultiTapDelay)
                {
                    // Cancel because time between multiple taps not respected
                    ResetGesture();
                }
            }
        }
        private void ResetGesture()
        {
            if (isBehaviourStarted)
            {
                OnGestureCanceled.Invoke(tapPosition);
            }

            isGestureStarted = false;
            touchStarted = false;
            isBehaviourStarted = false;
            delta = Vector2.zero;
            startTime = 0;
            tapCount = 0;
        }
        #endregion

        #region Simulation Gesture
        private bool simulatedThisFrame = false;
        private bool simulatedLastFrame = false;
        private bool simulationStarted = false;
        private bool simulationBehaviourStarted = false;
        private Vector2 simulatedGlobalDelta;
        private float simulatedStartTime;
        private int simulatedTapCount = 0;

        private int simulatedTouchCount = 0;
        private int simulatedOldTouchCount = 0;
        private Vector2 simulatedTouchPosition;
        private Vector2 simulatedTouchDelta;
        internal override void UpdateGestureSimulation()
        {
            if (simulatedThisFrame && TestTouchRestriction(simulatedTouchCount))
            {
                if (!simulatedLastFrame && simulatedOldTouchCount < simulatedTouchCount) // Start tap 
                {
                    if (!simulationStarted)
                    {
                        simulationStarted = true;
                        simulatedTapCount = 0;
                    }
                    simulatedStartTime = Time.time;
                    simulatedGlobalDelta = Vector2.zero;
                }

                // Simulation
                if (simulationStarted)
                {
                    if (!simulationBehaviourStarted && Time.time - simulatedStartTime >= MinDuration)
                    {
                        OnGestureStarted.Invoke(simulatedTouchPosition);
                        simulationBehaviourStarted = true;
                    }
                    if (simulationBehaviourStarted)
                    {
                        if (Time.time - simulatedStartTime > MaxDuration)
                        {
                            ResetSimulation();
                        }
                        else
                        {
                            simulatedGlobalDelta += simulatedTouchDelta;
                            if (simulatedGlobalDelta.magnitude > MaxMouvementDistance)
                            {
                                ResetSimulation();
                            }
                        }
                    }
                }
                simulatedLastFrame = true;
            }
            else if (simulatedLastFrame) // End tap
            {
                if (simulationBehaviourStarted && simulationStarted)
                {
                    simulatedTapCount++;
                    if (simulatedTapCount == MultiTapCountToTrigger)
                    {
                        OnGesturePerformed.Invoke(simulatedTouchPosition);
                        ResetSimulation();
                    }
                    else
                    {
                        simulatedGlobalDelta = Vector2.zero;
                        simulatedStartTime = Time.time;
                    }
                }
                simulatedLastFrame = false;
                simulatedTouchCount = 0;
            }
            else // Off simulation
            {
                if (Time.time - simulatedStartTime > MaxMultiTapDelay && simulationStarted)
                {
                    // Cancel because time between multiple taps not respected
                    ResetSimulation();
                }
                simulatedLastFrame = false;
                simulatedTouchCount = 0;
            }
            simulatedThisFrame = false;
            simulatedOldTouchCount = simulatedTouchCount;
        }
        public void SimulateTap(int nbTouch, Vector2 position, Vector2 deltaPosition)
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
                OnGestureCanceled.Invoke(simulatedTouchPosition);
            }
            simulationStarted = false;
            simulationBehaviourStarted = false;
            simulatedGlobalDelta = Vector2.zero;
            simulatedTapCount = 0;
            simulatedStartTime = 0;
        }
        #endregion

        private bool isTouchEndingThisFrame(UnityEngine.InputSystem.EnhancedTouch.Touch[] touches)
        {
            foreach (var touch in touches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended) return false;
            }
            return true;
        }

#if UNITY_EDITOR
        [MenuItem("Virtual-IT/TouchInputs/Create Tap gesture...", priority = 0)]
        internal static void CreateScriptableObject()
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            string directory = (string)args[0];

            TapTouchGesture newLogger = CreateInstance<TapTouchGesture>();
            AssetDatabase.CreateAsset(newLogger, AssetDatabase.GenerateUniqueAssetPath(directory + "/TapTouchGesture.asset"));
            Selection.activeObject = newLogger;
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
