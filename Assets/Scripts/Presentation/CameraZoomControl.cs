using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField]
    [Tooltip("How fast the camera zooms in/out per scroll tick")]
    private float zoomSpeed = 5f;
    [SerializeField]
    [Tooltip("Minimum orthographic size (zoom in)")]
    private float minSize = 2f;
    [SerializeField]
    [Tooltip("Maximum orthographic size (zoom out)")]
    private float maxSize = 20f;
    [SerializeField]
    [Tooltip("Smooth time for zooming")]
    private float smoothTime = 0.08f;

    private float zoomVelocity;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("CameraControl: no Camera found on the GameObject and no Camera assigned.");
            enabled = false;
            return;
        }

        if (!cam.orthographic)
        {
            Debug.LogWarning("CameraControl: assigned camera is not orthographic. Switching to orthographic.");
            cam.orthographic = true;
        }
    }

    private void LateUpdate()
    {
        float scroll;
#if ENABLE_INPUT_SYSTEM
        // Using the new Input System
        if (Mouse.current != null)
            scroll = Mouse.current.scroll.y.ReadValue();
        else
            scroll = 0f;
#else
        // Using legacy Input Manager
        scroll = Input.GetAxis("Mouse ScrollWheel");
#endif

        if (Mathf.Approximately(scroll, 0f))
            return;

        float targetSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minSize, maxSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, smoothTime);
    }
}
