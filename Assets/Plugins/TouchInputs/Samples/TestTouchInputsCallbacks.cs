using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualIT.TouchInputs;

public class TestTouchInputsCallbacks : MonoBehaviour
{
    [SerializeField] private List<BaseTouchGesture> gesturesToTest= new List<BaseTouchGesture>();
    private void Start()
    {
        foreach (BaseTouchGesture gesture in gesturesToTest)
        {
            gesture.EnableGesture();

            TapTouchGesture tapGesture = gesture as TapTouchGesture;
            if (tapGesture != null)
            {
                tapGesture.OnGestureStarted.AddListener(ctx => Debug.Log("Tap started: " + ctx));
                tapGesture.OnGesturePerformed.AddListener(ctx => Debug.Log("Tap performed: " + ctx));
                tapGesture.OnGestureCanceled.AddListener(ctx => Debug.Log("Tap canceled: " + ctx));
            }
            SlideTouchGesture slideGesture = gesture as SlideTouchGesture;
            if (slideGesture != null)
            {
                slideGesture.OnGestureStarted.AddListener(ctx => Debug.Log("Slide started: " + ctx.CurrentPosition));
                slideGesture.OnGesturePerformed.AddListener(ctx => Debug.Log("Slide performed: " + ctx.CurrentPosition));
                slideGesture.OnGestureCanceled.AddListener(ctx => Debug.Log("Slide canceled: " + ctx.CurrentPosition));
            }
            StretchTouchGesture stretchGesture = gesture as StretchTouchGesture;
            if (stretchGesture != null)
            {
                stretchGesture.OnGestureStarted.AddListener(ctx => Debug.Log("Stretch started: " + ctx.Stretch));
                stretchGesture.OnGesturePerformed.AddListener(ctx => Debug.Log("Stretch performed: " + ctx.Stretch));
                stretchGesture.OnGestureCanceled.AddListener(ctx => Debug.Log("Stretch canceled: " + ctx.Stretch));
            }
            RotateTouchGesture rotateGesture = gesture as RotateTouchGesture;
            if (rotateGesture != null)
            {
                rotateGesture.OnGestureStarted.AddListener(ctx => Debug.Log("Rotation started: " + ctx.Rotation));
                rotateGesture.OnGesturePerformed.AddListener(ctx => Debug.Log("Rotation performed: " + ctx.Rotation));
                rotateGesture.OnGestureCanceled.AddListener(ctx => Debug.Log("Rotation canceled: " + ctx.Rotation));
            }
        }
    }
}
