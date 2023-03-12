using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualIT.TouchInputs;

public class SquadManagerControl : MonoBehaviour
{
    [Tooltip("Gesture settings to handle the joystick control.")]
    [SerializeField] private SlideTouchGesture SlideTouchGesture;

    public RectTransform joystickBackground;
    public RectTransform joystickMove;
    
    
    private Vector2 positionTargetJoystickMove;

    [SerializeField]
    private float sizePossibleJoystickMax = 100f;
    private bool isPressingJoystick = false;

    //Output value of the joystick.
    private Vector2 orientationJoystick;
    private float magnitudeJoystick;

    // Start is called before the first frame update
    void Awake()
    {
        //Initialising the gesture for the joystick control.
        SlideTouchGesture.EnableGesture();
        SlideTouchGesture.OnGestureStarted.AddListener(ctx => StartControlJoystick(ctx.StartPosition));
        SlideTouchGesture.OnGesturePerformed.AddListener(ctx => ControlJoystick(ctx.CurrentPosition, ctx.StartPosition));
        SlideTouchGesture.OnGestureCanceled.AddListener(ctx => EndControlJoystick(ctx.CurrentPosition));
    }

    // Update is called once per frame
    void Update()
    {
        //Updating the position of the joystick image to give feedback with the position of finger.
        joystickMove.anchoredPosition = Vector2.Lerp(joystickMove.anchoredPosition, positionTargetJoystickMove, isPressingJoystick ? 50 * Time.deltaTime : 5 * Time.deltaTime);
    }

    /// <summary>
    /// Initialize the joystick data when the user starts touching the screen.
    /// </summary>
    void StartControlJoystick(Vector2 startPosition)
    {
        isPressingJoystick = true;

        joystickBackground.anchoredPosition = ((startPosition - Screen.width*0.5f*Vector2.right) / new Vector2(Screen.width, Screen.height)) * this.GetComponent<RectTransform>().sizeDelta;
    }

    /// <summary>
    /// Reset the joystick data when the user stops touching the screen.
    /// </summary>
    void EndControlJoystick(Vector2 startPosition)
    {
        isPressingJoystick = false;
        positionTargetJoystickMove = Vector2.zero;
        magnitudeJoystick = 0f;
        orientationJoystick = Vector2.zero;
    }

    /// <summary>
    /// Update the joystick data when the user keeps touching the screen.
    /// </summary>
    void ControlJoystick(Vector2 currentPosition, Vector2 startingPosition)
    {
        if(isPressingJoystick)
        {
            float angleJoystick = Mathf.Atan2(currentPosition.y - startingPosition.y, currentPosition.x - startingPosition.x);
            float distance = (currentPosition - startingPosition).magnitude;
            if (distance > sizePossibleJoystickMax) distance = sizePossibleJoystickMax;

            orientationJoystick = new Vector2(Mathf.Cos(angleJoystick), Mathf.Sin(angleJoystick));

            positionTargetJoystickMove = distance * orientationJoystick;

            magnitudeJoystick = distance / sizePossibleJoystickMax;

        }
    }

    /// <summary>
    /// Return the normalized vector2 of the direction of the joystick.
    /// </summary>
    public Vector2 GetInputJoystickDirection()
    {
        return this.orientationJoystick;
    }

    /// <summary>
    /// Return the normalized value between 0 and 1 of the distance between the joystick and the 0 position of it.
    /// </summary>
    public float GetInputJoystickMagnitude()
    {
        return this.magnitudeJoystick;
    }

}
