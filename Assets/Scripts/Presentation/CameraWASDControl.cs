using UnityEngine;
using UnityEngine.InputSystem;

public class CameraWASDControl : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField]
    [Tooltip("Movement speed in world units per second")]
    private float moveSpeed;
    [SerializeField]
    [Tooltip("Smooth time for movement")]
    private float smoothTime = 0.08f;

    private Vector3 currentVelocity;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("CameraWASDControl: no Camera found or assigned.");
            enabled = false;
        }
    }

    private void Update()
    {
        Vector2 input = GetInputVector();
        if (input.sqrMagnitude < Mathf.Epsilon)
            return;

        // Move on X and Y world axes (use camera right and camera up projected onto XY).
        Vector3 right = Vector3.Scale(cam.transform.right, new Vector3(1f, 1f, 0f));
        Vector3 up = Vector3.Scale(cam.transform.up, new Vector3(1f, 1f, 0f));

        if (right.sqrMagnitude < Mathf.Epsilon) right = Vector3.right;
        else right.Normalize();

        if (up.sqrMagnitude < Mathf.Epsilon) up = Vector3.up;
        else up.Normalize();

        Vector3 delta = (right * input.x + up * input.y) * moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    private Vector2 GetInputVector()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            Vector2 v = new Vector2(x, y);
            return v.sqrMagnitude > 1f ? v.normalized : v;
        }
        return Vector2.zero;
#else
        float x = 0f;
        float y = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) y -= 1f;
        Vector2 v = new Vector2(x, y);
        return v.sqrMagnitude > 1f ? v.normalized : v;
#endif
    }
}
