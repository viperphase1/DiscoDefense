using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class ArmSwingVelocity : MonoBehaviour {
    public GameObject xrRig;
    public GameObject mainCamera;
    public GameObject leftControllerObj;
    public GameObject rightControllerObj;

    public float speedFactor = 100;
    public int frameSampleSize = 3;

    private Vector3 xrRigPosition;
    private Vector3 leftControllerPosition;
    private Vector3 rightControllerPosition;

    private Vector3 lastFrameXrRigPosition;
    private Vector3 lastFrameLeftControllerPosition;
    private Vector3 lastFrameRightControllerPosition;

    private List<InputDevice> controllers = new List<InputDevice>();
    private InputDevice leftController;
    private InputDevice rightController;

    private Rigidbody rb;
    
    private List<float> handSpeeds = new List<float>();

    bool GetControllers() {
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        if (controllers.Count == 2) {
            foreach (var controller in controllers) {
                if ((controller.characteristics & InputDeviceCharacteristics.Right) == InputDeviceCharacteristics.Right) {
                    rightController = controller;
                } else {
                    leftController = controller;
                }
            }
            return true;
        }
        return false;
    }
    
    // Start is called before the first frame update
    void Start() {
        rb = xrRig.GetComponent<Rigidbody>();
        lastFrameXrRigPosition = xrRig.transform.position;
        lastFrameLeftControllerPosition = leftControllerObj.transform.position;
        lastFrameRightControllerPosition = rightControllerObj.transform.position;
        GetControllers();
    }

    // Update is called once per frame
    void Update() {
        // TODO: return early if the level/round hasn't started yet
        if (controllers.Count < 2) {
            if (!GetControllers()) {
                return;
            }
        }

        leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTrigger);
        rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger);

        // Debug.Log("Left trigger amount: " + leftTrigger);
        // Debug.Log("Right trigger amount: " + rightTrigger);

        // run the following only when the player is holding the back triggers

        xrRigPosition = xrRig.transform.position;
        leftControllerPosition = leftControllerObj.transform.position;
        rightControllerPosition = rightControllerObj.transform.position;

        float handSpeed = 0.0f;

        if (leftTrigger > 0.8 && rightTrigger > 0.8) {
            float xrRigDisplacement = Vector3.Distance(xrRigPosition, lastFrameXrRigPosition);
            float leftHandDisplacement = Vector3.Distance(leftControllerPosition, lastFrameLeftControllerPosition);
            float rightHandDisplacement = Vector3.Distance(rightControllerPosition, lastFrameRightControllerPosition);

            handSpeed = (leftHandDisplacement - xrRigDisplacement) + (rightHandDisplacement - xrRigDisplacement);
        }

        handSpeeds.Add(handSpeed);

        float avg = 0;
        if (handSpeeds.Count > 0) {
            avg = Queryable.Average(handSpeeds.AsQueryable());
        }

        // Debug.Log($"Average speed over last {frameSampleSize} frames: {avg}");

        // TODO: allow going backwards by swinging counter clockwise
        Vector3 forwardVector = new Vector3(mainCamera.transform.forward.x, mainCamera.transform.forward.y, mainCamera.transform.forward.z);
        forwardVector.y = 0;
        forwardVector.Normalize();

        Vector3 velocity = forwardVector * avg * speedFactor;

        rb.velocity = velocity;

        while (handSpeeds.Count >= frameSampleSize) {
            handSpeeds.RemoveAt(0);
        }

        lastFrameXrRigPosition = xrRigPosition;
        lastFrameLeftControllerPosition = leftControllerPosition;
        lastFrameRightControllerPosition = rightControllerPosition;
    }
}
