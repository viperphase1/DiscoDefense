using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        setRotationFromCamera();
    }

    // Update is called once per frame
    void Update()
    {
        setRotationFromCamera();
    }

    void setRotationFromCamera() {
        Vector3 cameraRotation = camera.rotation.eulerAngles;

        // Get the current rotation of this object
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Copy y rotation from the source object
        currentRotation.y = cameraRotation.y;

        // Apply the new rotation to this object
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
