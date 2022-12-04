using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ArmSwingMovement : MonoBehaviour {
    public GameObject xrRig;
    public GameObject mainCamera;
    public GameObject leftControllerObj;
    public GameObject rightControllerObj;

    public float speedFactor = 100;

    private Vector3 xrRigPosition;
    private Vector3 leftControllerPosition;
    private Vector3 rightControllerPosition;

    private Vector3 lastFrameXrRigPosition;
    private Vector3 lastFrameLeftControllerPosition;
    private Vector3 lastFrameRightControllerPosition;

    private List<Collision> collisions = new List<Collision>();

    private List<InputDevice> controllers = new List<InputDevice>();
    private InputDevice leftController;
    private InputDevice rightController;

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
        lastFrameXrRigPosition = xrRig.transform.position;
        lastFrameLeftControllerPosition = leftControllerObj.transform.position;
        lastFrameRightControllerPosition = rightControllerObj.transform.position;
        GetControllers();
    }

    Vector3 getCollisionSurfaceNormal(Collision collision) {
        List<ContactPoint> contactPoints = new List<ContactPoint>();
        collision.GetContacts(contactPoints);
        Vector3 p1 = contactPoints[0].point;
        Vector3 p2 = contactPoints[1].point;
        Vector3 p3 = contactPoints[2].point;
        Vector3 cross = Vector3.Cross(p2 - p1, p3 - p1);
        Vector3 normal = Vector3.Normalize(cross);
        Ray contactRay = new Ray(p1, normal);
        // check if normal is facing into the wall (this only works on 3d objects)
        // reverse if it is
        if (collision.collider.bounds.Contains(p1 + normal * 0.001f)) {
            Debug.Log("reverse normal");
            normal *= -1;
        }
        return normal;
    }

    bool forbiddenDirection(Vector3 dir, Vector3 normal) {
        Vector3 projection = Vector3.Project(dir, normal);
        return Vector3.Normalize(projection) == normal * -1;
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

        if (leftTrigger > 0.8 && rightTrigger > 0.8) {
            float xrRigDisplacement = Vector3.Distance(xrRigPosition, lastFrameXrRigPosition);
            float leftHandDisplacement = Vector3.Distance(leftControllerPosition, lastFrameLeftControllerPosition);
            float rightHandDisplacement = Vector3.Distance(rightControllerPosition, lastFrameRightControllerPosition);

            float handSpeed = (leftHandDisplacement - xrRigDisplacement) + (rightHandDisplacement - xrRigDisplacement);

            Vector3 forwardVector = new Vector3(mainCamera.transform.forward.x, mainCamera.transform.forward.y, mainCamera.transform.forward.z);
            // only allow movement in the x and z directions (player should be always be grounded)
            forwardVector.y = 0;
            // we don't want the y component of the forward vector to affect the movement speed
            forwardVector.Normalize();

            // TODO: NEW APPROACH
            // you can simplify this by first calculating the proposed movement vector 
            // then projecting it onto the normal of the collision plane
            // if the projection is a nonzero factor of the plane's inverted normal (let's call this being "negative") set the proposed movement vector to its projection on the collision plane
            // if colliding with two or more walls prevent all movement if the proposed movement vector's projection on the normal of two different collision planes is "negative"

            Vector3 proposedMovement = Vector3.zero;

            if (Time.timeSinceLevelLoad > 1f) {
                proposedMovement = (forwardVector * handSpeed * speedFactor * Time.deltaTime);
            }

            int wallBreaks = 0;

            for (int i = 0; i < collisions.Count; i++) {
                Vector3 n = getCollisionSurfaceNormal(collisions[i]);
                Debug.Log($"Collision plane normal ({n.x},{n.y},{n.z})");
                Debug.Log($"Proposed movement ({proposedMovement.x},{proposedMovement.y},{proposedMovement.z})");
                if (forbiddenDirection(proposedMovement, n)) {
                    wallBreaks++;
                    Debug.Log($"Wall break predicted. Number of wall breaks at this time ({wallBreaks})");
                    if (wallBreaks > 1) {
                        Debug.Log("Multiple wall breaks, aborting movement");
                        proposedMovement = Vector3.zero;
                        break;
                    }
                    Debug.Log("Project movement on plane");
                    proposedMovement = Vector3.ProjectOnPlane(proposedMovement, n);
                }
            }

            xrRig.transform.position += proposedMovement;

        }

        lastFrameXrRigPosition = xrRigPosition;
        lastFrameLeftControllerPosition = leftControllerPosition;
        lastFrameRightControllerPosition = rightControllerPosition;
    }

    void OnCollisionEnter(Collision collision) {
        if (!collisions.Contains(collision)) {
            Debug.Log("Added collision for " + collision.gameObject.name);
            collisions.Add(collision);
        }
    }

    void OnCollisionExit(Collision collision) {
        Debug.Log("Collision resolved for " + collision.gameObject.name);
        for (int i = 0; i < collisions.Count; i++) {
            if (collisions[i].gameObject == collision.gameObject) {
                collisions.Remove(collisions[i]);
            }
        }
    }
}
