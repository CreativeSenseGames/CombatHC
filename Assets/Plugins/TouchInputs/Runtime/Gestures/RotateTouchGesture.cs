using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualIT.TouchInputs
{
    public class RotateTouchGesture : BaseTouchGesture
    {
        [Header("Rotate properties")]
        [Tooltip("Minimum delta angle that can be done to be triggered")]
        public float MinAngleDistance = 1;
        [Space(10)]
        public UnityEvent<RotateTouchData> OnGestureStarted;
        public UnityEvent<RotateTouchData> OnGesturePerformed;
        public UnityEvent<RotateTouchData> OnGestureCanceled;

        private RotateTouchData rotateData = new RotateTouchData();
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
                rotateData = new RotateTouchData();
                rotateData.Rotation = 0;
            }


            if (isGestureStarted) // If touches are tracked by Gesture
            {
                rotateData.RotateCenterPosition = GetAveragePosition(touches);
                rotateData.FrameRotation = 0;
                foreach (var touch in touches)
                {
                    rotateData.FrameRotation += Vector2.SignedAngle(touch.screenPosition - rotateData.RotateCenterPosition, touch.screenPosition - touch.delta - rotateData.RotateCenterPosition) / touches.Length;
                }
                rotateData.Rotation += rotateData.FrameRotation;


                if (!isGestureBehaviourStarted && Time.time - startTime > MinDuration && Mathf.Abs(rotateData.Rotation) > MinAngleDistance) // Start gesture condition
                {
                    isGestureBehaviourStarted = true;
                    OnGestureStarted.Invoke(rotateData);
                }
                if (isGestureBehaviourStarted) // Update gesture
                {
                    OnGesturePerformed.Invoke(rotateData);

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
                OnGestureCanceled.Invoke(rotateData);
            }

            isGestureStarted = false;
            isGestureBehaviourStarted = false;

            startTime = 0;
            rotateData = new RotateTouchData();
            rotateData.Rotation = 0;
        }

        public class RotateTouchData
        {
            public Vector2 RotateCenterPosition;
            public float Rotation;
            public float FrameRotation;
        }
        public void SimulateRotation()
        {

            simulatedThisFrame = true;
        }

#if UNITY_EDITOR
        [MenuItem("Virtual-IT/TouchInputs/Create Rotate gesture...", priority = 3)]
        internal static void CreateScriptableObject()
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            string directory = (string)args[0];

            RotateTouchGesture newLogger = CreateInstance<RotateTouchGesture>();
            AssetDatabase.CreateAsset(newLogger, AssetDatabase.GenerateUniqueAssetPath(directory + "/RotateTouchGesture.asset"));
            Selection.activeObject = newLogger;
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
