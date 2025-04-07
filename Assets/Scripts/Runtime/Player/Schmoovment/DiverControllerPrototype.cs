using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DiverControllerPrototype : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;

    [Header("Kick Settings")]
    public float kickForce = 5f;

    public float kickDuration = 1f;
    public AnimationCurve kickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Buoyancy Settings")]
    public float bcdForce = 3f;

    public float surfaceY = 0f; // Adjustable "sea level"
    public float pressureMultiplier = 0.1f;
    public float maxVerticalSpeed = 2f;

    private Rigidbody rb;
    private Coroutine kickRoutine;
    private bool isCursorLocked = false;
    //private Vector2 inputCords = new Vector2(0, 0);

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RotateTowardMouse();

        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLocked;
        }

        if (Input.GetKeyDown(KeyCode.W) && kickRoutine == null)
        {
            kickRoutine = StartCoroutine(KickCoroutine());
        }
    }

    private void FixedUpdate()
    {
        HandleBuoyancy();
    }

    // Rotates the diver to face the mouse cursor (in 2D, circular side-on)
    private void RotateTowardMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Applies forward thrust over time, like a diver's kick
    private IEnumerator KickCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < kickDuration)
        {
            float forceMultiplier = kickCurve.Evaluate(elapsed / kickDuration);
            Vector3 force = transform.up * kickForce * forceMultiplier;

            rb.AddForce(force, ForceMode.Acceleration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        kickRoutine = null;
    }

    // Adjusts vertical position based on BCD and pressure at depth
    private void HandleBuoyancy()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.LeftShift)) input = 1f;
        else if (Input.GetKey(KeyCode.LeftControl)) input = -1f;

        if (input != 0f)
        {
            float depth = Mathf.Abs(transform.position.y - surfaceY);
            float pressure = 1f + depth * pressureMultiplier;

            float adjustedForce = (bcdForce / pressure) * input;

            if (Mathf.Abs(rb.linearVelocity.y) < maxVerticalSpeed)
            {
                rb.AddForce(Vector3.up * adjustedForce, ForceMode.Acceleration);
            }
        }
    }

#if UNITY_EDITOR

    // Debug Gizmos for inspector visualization
    private void OnDrawGizmos()
    {
        // Rotation direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);

        // Surface line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-100f, surfaceY, 0f), new Vector3(100f, surfaceY, 0f));
    }

#endif
}