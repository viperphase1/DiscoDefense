using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TapTrigger : MonoBehaviour
{
    public InputActionProperty rightTriggerAction;
    private bool isPressed = false;
    private float pressStartTime;
    private float quickTapThreshold = 0.2f; // Adjust this threshold as needed

    void OnEnable()
    {
        rightTriggerAction.action.Enable();
        rightTriggerAction.action.performed += OnTriggerPressed;
        rightTriggerAction.action.canceled += OnTriggerReleased;
    }

    void OnDisable()
    {
        rightTriggerAction.action.performed -= OnTriggerPressed;
        rightTriggerAction.action.canceled -= OnTriggerReleased;
        rightTriggerAction.action.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        isPressed = true;
        pressStartTime = Time.time;
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (isPressed)
        {
            float pressDuration = Time.time - pressStartTime;
            if (pressDuration <= quickTapThreshold)
            {
                // Quick tap detected
                Debug.Log("Quick tap detected!");
                // Add your quick tap handling code here
            }
            isPressed = false;
        }
    }
}