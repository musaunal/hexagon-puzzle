using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{

    // Mobile Swipe Controls
    private bool tap, swipeRight, swipeLeft;
    private bool isDraging = false;
    private bool moved;
    private Vector2 startTouch, swipeDelta;

    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool Tap { get { return tap; } }


    void Update()
    {
        ResetInput();

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            //tap = true;
            isDraging = true;
            moved = false;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }
        #endregion

        #region Mobile Inputs
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                //tap = true;
                isDraging = true;
                moved = false;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }

        #endregion

        #region Calculate delta

        // Calculate the distance
        swipeDelta = Vector2.zero;
        // Player touches the screen
        if (isDraging)
        {
            // We started the touch somewhere
            if (Input.touchCount > 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        // Did we cross the deadzone?
        // Deadzone in pixels
        if (swipeDelta.magnitude > 25)
        {
            moved = true;

            // Which direction are we swiping in?
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (x < 0)
            {
                swipeLeft = true;
            }
            else
            {
                swipeRight = true;
            }

            Reset();
        }

        #endregion

        #region Process the bools and execute the action
        if (swipeRight)
        {
            // MY PRIVATE CODE
            // INSERT ANIMATIONS / TRANSFORMS / FORCES HERE
        }

        if (swipeLeft)
        {
            // MY PRIVATE CODE
            // INSERT ANIMATIONS / TRANSFORMS / FORCES HERE
        }

        if (tap)
        {
            // MY PRIVATE CODE
            // INSERT ANIMATIONS / TRANSFORMS / FORCES HERE
        }
        #endregion

    }


    private void Reset()
    {
        if (!moved) tap = true;

        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }

    private void ResetInput()
    {
        tap = swipeLeft = swipeRight = false;
    }


}