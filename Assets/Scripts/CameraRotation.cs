using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 5.0f; // Controls how fast the camera rotates

    private float pitch = 0.0f; // Rotation around the x-axis
    private float yaw = 0.0f;   // Rotation around the y-axis

    void Update()
    {
        // Check if the right mouse button is being held down
        if (Input.GetMouseButton(1))
        {
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Adjust yaw and pitch based on mouse input
            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            // Clamp the pitch to avoid flipping the camera
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            // Apply rotation to the camera
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
