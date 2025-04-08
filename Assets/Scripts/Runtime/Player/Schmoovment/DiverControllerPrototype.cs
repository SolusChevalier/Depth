using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class DiverControllerPrototype : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 180f;

    [Header("Kick Settings")]
    public float kickForce = 5f;

    public float kickDuration = 0.5f;
    public float kickCooldown = 0.5f;
    public AnimationCurve kickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Buoyancy Settings")]
    public float bcdForce = 3f;

    public float surfaceY = 0f; // Adjustable "sea level"
    public float pressureMultiplier = 0.1f;
    public float maxVerticalSpeed = 2f;
    public float neutralTopY = -5f; // Top level where you are neutral with no air
    public float neutralBottomY = -20f; // Bottom level where you need full BCD to be neutral
    public float maxBcdAir = 100f; // Maximum BCD air amount

    public float currentBcdAir = 0f; // Current BCD air amount
    private Rigidbody rb;
    private Coroutine kickRoutine;
    private bool isCursorLocked = false;

    [Header("Diver Stats")]
    public float depth = 0f;

    public float pressure = 0f;

    [Header("Unity Events")]
    public UnityEvent<float> onDepthChanged;

    public UnityEvent<float> onPressureChanged;
    public UnityEvent<float> onBcdAirChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = !isCursorLocked;
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLocked;
        }

        if (isCursorLocked)
        {
            RotateWithKeyboard();
        }
        else
        {
            RotateTowardMouse();
        }

        if (Input.GetKeyDown(KeyCode.W) && kickRoutine == null)
        {
            kickRoutine = StartCoroutine(KickCoroutine());
        }
    }

    // Rotates the diver using A and D keys
    private void RotateWithKeyboard()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, 0f, -rotationAmount);
    }

    private void FixedUpdate()
    {
        float previousDepth = depth;
        float previousPressure = pressure;
        float previousBcdAir = currentBcdAir;

        // Normalize depth to 0–100
        depth = Mathf.InverseLerp(neutralBottomY, neutralTopY, transform.position.y - surfaceY) * 100f;

        // Normalize pressure to 0–100
        float rawPressure = Mathf.Lerp(1f, 1f + pressureMultiplier, Mathf.InverseLerp(neutralTopY, neutralBottomY, transform.position.y - surfaceY));
        pressure = Mathf.InverseLerp(1f, 1f + pressureMultiplier, rawPressure) * 100f;

        // Normalize currentBcdAir to 0–100
        float normalizedBcdAir = (currentBcdAir / maxBcdAir) * 100f;

        HandleBuoyancy();

        // Trigger Unity Events if values change
        if (Mathf.Abs(depth - previousDepth) > Mathf.Epsilon)
        {
            onDepthChanged?.Invoke(depth);
        }

        if (Mathf.Abs(pressure - previousPressure) > Mathf.Epsilon)
        {
            onPressureChanged?.Invoke(pressure);
        }

        if (Mathf.Abs(normalizedBcdAir - previousBcdAir) > Mathf.Epsilon)
        {
            onBcdAirChanged?.Invoke(normalizedBcdAir);
        }
    }

    // Rotates the diver to face the mouse cursor (in 2D, circular side-on)

    private void RotateTowardMouse()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 mousePosition = Input.mousePosition;

        // Calculate the direction from the center to the mouse position
        Vector3 direction = mousePosition - screenCenter;

        // Constrain the direction to a circle with a radius of 100 pixels
        float radius = 100f;
        if (direction.magnitude > radius)
        {
            direction = direction.normalized * radius;
        }

        // Calculate the constrained mouse position
        Vector3 constrainedMousePosition = screenCenter + direction;

        // Convert the constrained mouse position to world point
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(constrainedMousePosition);
        Vector2 directionToFace = (mouseWorld - transform.position);
        float angle = Mathf.Atan2(directionToFace.y, directionToFace.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Adjusts vertical position based on BCD and pressure at depth
    private void HandleBuoyancy()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentBcdAir = Mathf.Min(currentBcdAir + Time.deltaTime * 10f, maxBcdAir);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentBcdAir = Mathf.Max(currentBcdAir - Time.deltaTime * 10f, 0f);
        }

        // Apply buoyancy force based on current BCD air amount and pressure
        float buoyancyForce = CalculateBuoyancyForce(depth, pressure);
        rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
    }

    // Calculates the buoyancy force based on the diver's depth and pressure
    private float CalculateBuoyancyForce(float depth, float pressure)
    {
        float bcdEffectiveness = currentBcdAir / maxBcdAir;
        float adjustedBcdForce = bcdForce * bcdEffectiveness / pressure;

        if (currentBcdAir == 0f)
        {
            // Apply a gradual negative buoyancy based on depth and pressure
            float negativeBuoyancyForce = -bcdForce * Mathf.Lerp(0.1f, 1f, Mathf.InverseLerp(neutralTopY, neutralBottomY, depth)) / pressure;
            return negativeBuoyancyForce;
        }

        if (depth > neutralTopY)
        {
            return -adjustedBcdForce; // Negative buoyancy above neutral top
        }
        else if (depth < neutralBottomY)
        {
            return adjustedBcdForce; // Positive buoyancy below neutral bottom
        }
        else
        {
            // Interpolate buoyancy force within the neutral range
            float t = Mathf.InverseLerp(neutralBottomY, neutralTopY, depth);
            return Mathf.Lerp(adjustedBcdForce, -adjustedBcdForce, t);
        }
    }

    // Applies forward thrust over time, like a diver's kick
    private IEnumerator KickCoroutine()
    {
        while (Input.GetKey(KeyCode.W))
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

            yield return new WaitForSeconds(kickCooldown);
        }

        kickRoutine = null;
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

        // Neutral buoyancy range
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-100f, neutralTopY, 0f), new Vector3(100f, neutralTopY, 0f));
        Gizmos.DrawLine(new Vector3(-100f, neutralBottomY, 0f), new Vector3(100f, neutralBottomY, 0f));
    }

#endif
}