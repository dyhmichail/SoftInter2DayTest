using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraRightMouseControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField]
    [Tooltip("Movement speed in world units per second")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("Smooth time for movement")]
    private float smoothTime = 0.8f;

    private Vector3 currentVelocity;
    private Vector2 lastMousePosition;
    private bool isDragging = false;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("CameraRightMouseControl: no Camera found or assigned.");
            enabled = false;
        }
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // New Input System
        if (Mouse.current != null)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                isDragging = true;
                lastMousePosition = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector2 currentMousePos = Mouse.current.position.ReadValue();
                Vector2 deltaMouse = currentMousePos - lastMousePosition;

                Vector3 right = Vector3.Scale(cam.transform.right, new Vector3(1f, 1f, 0f));
                Vector3 up = Vector3.Scale(cam.transform.up, new Vector3(1f, 1f, 0f));

                if (right.sqrMagnitude < Mathf.Epsilon) right = Vector3.right;
                else right.Normalize();

                if (up.sqrMagnitude < Mathf.Epsilon) up = Vector3.up;
                else up.Normalize();

                Vector3 delta = (-right * deltaMouse.x - up * deltaMouse.y) * (moveSpeed * Time.deltaTime / 10f);
                Vector3 targetPosition = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

                lastMousePosition = currentMousePos;
            }
        }
#else
        // Legacy Input Manager
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 deltaMouse3 = Input.mousePosition - (Vector3)lastMousePosition;
            Vector2 deltaMouse = new Vector2(deltaMouse3.x, deltaMouse3.y);

            Vector3 right = Vector3.Scale(cam.transform.right, new Vector3(1f, 1f, 0f));
            Vector3 up = Vector3.Scale(cam.transform.up, new Vector3(1f, 1f, 0f));

            if (right.sqrMagnitude < Mathf.Epsilon) right = Vector3.right;
            else right.Normalize();

            if (up.sqrMagnitude < Mathf.Epsilon) up = Vector3.up;
            else up.Normalize();

            Vector3 delta = (-right * deltaMouse.x - up * deltaMouse.y) * (moveSpeed * Time.deltaTime / 10f);
            Vector3 targetPosition = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

            lastMousePosition = Input.mousePosition;
        }
#endif
    }
}