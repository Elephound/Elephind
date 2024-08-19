// 2024-08-19 AI-Tag 
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEngine.XR;

public class HeadFollowUI : MonoBehaviour
{
    public Transform vrCamera;
    public float followSpeed = 0.1f;

    public float distance = 2.0f;

    void Start()
    {
        if (vrCamera == null)
        {
            vrCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        Vector3 targetPosition = vrCamera.position + vrCamera.forward * distance; // Adjust distance as needed
        Quaternion targetRotation = Quaternion.Euler(vrCamera.eulerAngles.x, vrCamera.eulerAngles.y, 0);

        // Lerp for smooth transition with slight delay
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, followSpeed);
    }
}
