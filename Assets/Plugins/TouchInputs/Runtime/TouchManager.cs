using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

namespace VirtualIT.TouchInputs
{
    public class TouchManager : MonoBehaviour
    {
        private static TouchManager instance;
        public static TouchManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TouchManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject();
                        go.name = "[TouchManager]";
                        instance = go.AddComponent<TouchManager>();
                    }
                }
                return instance;
            }
        }

        private List<TouchGestureDefinition> touchGestures = new List<TouchGestureDefinition>();

        private void Awake()
        {
            EnhancedTouchSupport.Enable();
            SetupTouchGestureSimulation();
        }
        private void Update()
        {
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            // add touch reading historic in activeTouches

            if (activeTouches.Count == 0)
            {
                UpdateSimulation();
            }
            else
            {
                UpdateTouchVisualisation(activeTouches.ToArray());
            }

            foreach (var gesture in touchGestures)
            {
                gesture.gesture.UpdateGesture(activeTouches.Where(ctx => gesture.screen == null || ctx.screen == gesture.screen).ToArray());
                gesture.gesture.UpdateGestureSimulation();
            }
        }
        private void OnEnable()
        {
            EnableGestureSimultaion(true);
        }
        private void OnDisable()
        {
            EnableGestureSimultaion(false);
        }

        #region Touch Gestures Registering
        internal void EnableTouchGesture(BaseTouchGesture gesture, Touchscreen screen)
        {
            touchGestures.Add(new TouchGestureDefinition
            {
                gesture = gesture,
                screen = screen
            });
        }
        internal void DisableTouchGesture(BaseTouchGesture gesture, Touchscreen screen)
        {
            List<TouchGestureDefinition> touchGesturesToRemove = new List<TouchGestureDefinition>();
            touchGesturesToRemove = touchGestures.FindAll(ctx => ctx.gesture == gesture && (ctx.screen == null || ctx.screen == screen));
            foreach (TouchGestureDefinition gesturToRemove in touchGesturesToRemove)
            {
                touchGestures.Remove(gesturToRemove);
            }
        }
        #endregion

        #region Touch Visualisation 
        private Color touchVisualisationColor = Color.clear;
        private Canvas touchVisualisationCanvas = null;
        private Sprite circleSprite = null;
        private float touchVisualisationCircleSize = 100;
        private Dictionary<Finger, Image> touchVisualisationImages = new Dictionary<Finger, Image>();
        public void SetTouchVisualisation(bool activate, Color? drawColor = null, float circleSize = 100)
        {
            if (activate)
            {
                if (drawColor == null)
                {
                    touchVisualisationColor = Color.white;
                }
                else
                {
                    touchVisualisationColor = drawColor.Value;
                }
                touchVisualisationColor.a = 0.3f;
                touchVisualisationCircleSize = circleSize;
            }
            else
            {
                touchVisualisationColor = Color.clear;
            }
        }
        private void UpdateTouchVisualisation(UnityEngine.InputSystem.EnhancedTouch.Touch[] activeTouches) // To adapt with multi touch screen
        {
            if (touchVisualisationColor != Color.clear)
            {
                if (touchVisualisationCanvas == null) SetupDebugCanvas();
                foreach (var touch in activeTouches)
                {
                    // Draw debug touch
                    if (!touchVisualisationImages.ContainsKey(touch.finger)) touchVisualisationImages.Add(touch.finger, CreateNewTouchVisualisationImage());

                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended && touchVisualisationImages.ContainsKey(touch.finger))
                    {
                        Destroy(touchVisualisationImages[touch.finger].gameObject);
                        touchVisualisationImages.Remove(touch.finger);
                    }
                    else
                    {
                        touchVisualisationImages[touch.finger].rectTransform.position = touch.screenPosition;
                        touchVisualisationImages[touch.finger].color = touchVisualisationColor;
                    }
                }
            }
        }
        private void SetupDebugCanvas()
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvasGO.transform.SetParent(transform);
            touchVisualisationCanvas = canvasGO.AddComponent<Canvas>();
            touchVisualisationCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            int circleResolution = 32;
            Texture2D circleTexture = new Texture2D(circleResolution, circleResolution);
            for (int x = 0; x < circleResolution; x++)
            {
                for (int y = 0; y < circleResolution; y++)
                {
                    Vector2 center = new Vector2(circleResolution / 2, circleResolution / 2);
                    float distanceFromCenter = Vector2.Distance(new Vector2(x, y), center);
                    float maxDistance = circleResolution / 2;
                    if (distanceFromCenter > maxDistance)
                    {
                        circleTexture.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        circleTexture.SetPixel(x, y, Color.white);
                    }
                }
            }
            circleTexture.Apply();
            circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, circleResolution, circleResolution), new Vector2(0.5f, 0.5f));

        }
        private Image CreateNewTouchVisualisationImage()
        {
            GameObject imageGO = new GameObject("Image");
            Image image = imageGO.AddComponent<Image>();
            image.color = Color.white;
            image.sprite = circleSprite;
            image.rectTransform.SetParent(touchVisualisationCanvas.transform);
            image.rectTransform.sizeDelta = Vector2.one * touchVisualisationCircleSize;
            image.rectTransform.anchoredPosition = new Vector2(0, 0);
            image.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            return image;
        }
        #endregion

        #region Touch Gestures Simulation By InputAction
        private TouchSimulationInputs simulationInputs;
        private bool simpleClickPressed = false;
        private bool twoClickPressed = false;
        private bool threeClickPressed = false;
        private bool alternativeBehaviourPressed = false;
        private float scrollAxisValue = 0;
        private Vector2 pointerPosition = Vector2.zero;
        private Vector2 pointerDeltaPosition = Vector2.zero;
        private void SetupTouchGestureSimulation()
        {
            simulationInputs = new TouchSimulationInputs();
            simulationInputs.TouchSimulation.SimpleClick.performed += SimpleClickPerformed;
            simulationInputs.TouchSimulation.TwoFingerClick.performed += TwoClickPerformed;
            simulationInputs.TouchSimulation.ThreeFingerClick.performed += ThreeClickPerformed;
            simulationInputs.TouchSimulation.AlternativeBehaviour.performed += AlternativeKeyPerformed;
            simulationInputs.TouchSimulation.Zoom.performed += ScrollPerformed;
            simulationInputs.TouchSimulation.PointerPosition.performed += MouvementPerformed;
            simulationInputs.TouchSimulation.PointerDeltaPosition.performed += MouvementDeltaPerformed;
        }
        private void EnableGestureSimultaion(bool enable)
        {
            if (enable)
            {
                simulationInputs.Enable();
            }
            else
            {
                simulationInputs.Disable();
            }
        }
        private void UpdateSimulation()
        {
            if (simpleClickPressed || twoClickPressed || threeClickPressed)
            {
                int fingerCount = simpleClickPressed ? 1 : twoClickPressed ? 2 : threeClickPressed ? 3 : 0;
                if (alternativeBehaviourPressed) fingerCount = 5;

                foreach (var gesture in touchGestures)
                {
                    SlideTouchGesture slideGesture = gesture.gesture as SlideTouchGesture;
                    if (slideGesture != null && slideGesture.EnableSimulation)
                    {
                        slideGesture.SimulateSlide(fingerCount, pointerPosition, pointerDeltaPosition);
                    }
                    TapTouchGesture staticGesture = gesture.gesture as TapTouchGesture;
                    if (staticGesture != null && staticGesture.EnableSimulation)
                    {
                        staticGesture.SimulateTap(fingerCount, pointerPosition, pointerDeltaPosition);
                    }
                }
            }
            else if (scrollAxisValue != 0)
            {
                if (alternativeBehaviourPressed)
                {
                    foreach (var gesture in touchGestures)
                    {
                        RotateTouchGesture rotateGesture = gesture.gesture as RotateTouchGesture;
                        if (rotateGesture != null && rotateGesture.EnableSimulation)
                        {
                            rotateGesture.SimulateRotation();
                        }
                    }
                    //Debug.Log("Rotate " + simulationInputs.TouchSimulation.Zoom.ReadValue<float>()); // Value de modif sensi
                }
                else
                {
                    foreach (var gesture in touchGestures)
                    {
                        StretchTouchGesture stretchGesture = gesture.gesture as StretchTouchGesture;
                        if (stretchGesture != null && stretchGesture.EnableSimulation)
                        {
                            stretchGesture.SimulateStretch();
                        }
                    }
                    //Debug.Log("Stretch " + simulationInputs.TouchSimulation.Zoom.ReadValue<float>()); // Value de modif sensi
                }
            }
        }
        private void SimpleClickPerformed(InputAction.CallbackContext context)
        {
            simpleClickPressed = context.ReadValue<float>() != 0;
        }
        private void TwoClickPerformed(InputAction.CallbackContext context)
        {
            twoClickPressed = context.ReadValue<float>() != 0;
        }
        private void ThreeClickPerformed(InputAction.CallbackContext context)
        {
            threeClickPressed = context.ReadValue<float>() != 0;
        }
        private void AlternativeKeyPerformed(InputAction.CallbackContext context)
        {
            alternativeBehaviourPressed = context.ReadValue<float>() != 0;
        }
        private void ScrollPerformed(InputAction.CallbackContext context)
        {
            scrollAxisValue = context.ReadValue<float>();
        }
        private void MouvementPerformed(InputAction.CallbackContext context)
        {
            pointerPosition = context.ReadValue<Vector2>();
        }
        private void MouvementDeltaPerformed(InputAction.CallbackContext context)
        {
            pointerDeltaPosition = context.ReadValue<Vector2>();
        }
        #endregion

        private class TouchGestureDefinition
        {
            public BaseTouchGesture gesture;
            public Touchscreen screen;
        }
    }
}
