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

    [Header("Rotation Inertia")]
    public float rotationAcceleration = 5f;

    public float rotationDamping = 3f;
    private float currentRotationInput = 0f;

    [Header("Neutral Buoyancy Tuning")]
    public float neutralAirThresholdLow = 45f;

    public float neutralAirThresholdHigh = 55f;

    [Header("Buoyancy Settings")]
    public float bcdForce = 4.5f;

    public float buoyancyBoostMultiplier = 3.5f;
    public float pressureMultiplier = 0.1f;
    public float maxVerticalSpeed = 2f;
    public float maxBcdAir = 100f; // Maximum BCD air amount

    public float currentBcdAir = 25f; // Current BCD air amount
    private Rigidbody rb;
    private Coroutine kickRoutine;
    private bool isCursorLocked = false;
    private float pressureAtDepth = 1f; // default surface pressure

    [Header("Map Bounds")]
    public Vector2 verticalBounds = new Vector2(0f, -30f); // (surfaceY, bottomY)

    // Calculated automatically:
    private float surfaceY => verticalBounds.x;

    private float bottomY => verticalBounds.y;
    private float neutralTopY => Mathf.Lerp(surfaceY, bottomY, 0.25f);   // 25% depth
    private float neutralBottomY => Mathf.Lerp(surfaceY, bottomY, 0.75f); // 75% depth

    [Header("Diver Stats")]
    public float depth = 0f;

    public float pressure = 0f;

    [Header("UI")]
    [Range(0, 100)]
    public float currentBuoyancyLevel = 50f; // 0 = max sink, 100 = max float, 50 = neutral

    public UnityEvent<float> onBuoyancyLevelChanged; // optional for UI binding

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
        if (Input.GetKeyUp(KeyCode.W) && kickRoutine != null)
        {
            StopCoroutine(kickRoutine);
            kickRoutine = null;
        }
    }

    // Rotates the diver using A and D keys
    private void RotateWithKeyboard()
    {
        float targetInput = -Input.GetAxis("Horizontal"); // Inverted to match original direction
        currentRotationInput = Mathf.MoveTowards(
            currentRotationInput,
            targetInput,
            rotationAcceleration * Time.deltaTime
        );

        // Apply damping when no input
        if (Mathf.Approximately(targetInput, 0f))
        {
            currentRotationInput = Mathf.MoveTowards(
                currentRotationInput,
                0f,
                rotationDamping * Time.deltaTime
            );
        }

        float rotationAmount = currentRotationInput * rotationSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, 0f, rotationAmount);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void FixedUpdate()
    {
        pressureAtDepth = GetPressureAtDepth();
        float previousDepth = depth;
        float previousPressure = pressure;
        float previousBcdAir = currentBcdAir;

        // Normalize depth to 0–100
        depth = Mathf.InverseLerp(bottomY, surfaceY, transform.position.y) * 100f;

        // Normalize pressure to 0–100
        float rawPressure = pressureAtDepth;
        pressure = Mathf.InverseLerp(1f, 1f + pressureMultiplier, rawPressure) * 100f;

        // Normalize currentBcdAir to 0–100
        float normalizedBcdAir = (currentBcdAir / maxBcdAir) * 100f;

        HandleBuoyancy();
        ApplyPassiveDrift();
        UpdateBuoyancyLevel(pressureAtDepth);

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

    private void ApplyPassiveDrift()
    {
        float normalizedBcd = (currentBcdAir / maxBcdAir) * 100f;

        if (normalizedBcd >= neutralAirThresholdLow && normalizedBcd <= neutralAirThresholdHigh)
        {
            // In the "float zone", barely move
            return;
        }

        // Sinking logic if outside float zone
        float driftForce;

        if (normalizedBcd < neutralAirThresholdLow)
        {
            // Below neutral, sinking
            float t = Mathf.InverseLerp(0f, neutralAirThresholdLow, normalizedBcd);
            driftForce = Mathf.Lerp(-1f, -0.1f, t);
        }
        else
        {
            // Above neutral, rising
            float t = Mathf.InverseLerp(neutralAirThresholdHigh, 100f, normalizedBcd);
            driftForce = Mathf.Lerp(0.1f, 1f, t);
        }

        rb.AddForce(Vector3.up * driftForce, ForceMode.Acceleration);
    }

    private void UpdateBuoyancyLevel(float pressureAtDepth)
    {
        if (currentBcdAir <= 0f)
        {
            currentBuoyancyLevel = 0f;
            onBuoyancyLevelChanged?.Invoke(currentBuoyancyLevel);
            return;
        }

        float effectiveVolume = (currentBcdAir / pressureAtDepth) * buoyancyBoostMultiplier;
        float normalizedLift = effectiveVolume / maxBcdAir; // 0.0 to 1.0

        // Map effective buoyancy to a scale where:
        // 0   = max sink
        // 50  = neutral
        // 100 = max float

        float centerNeutral = 0.5f; // mid-point is 50%
        float buoyancyOffset = normalizedLift - centerNeutral;

        currentBuoyancyLevel = Mathf.Clamp01(centerNeutral + buoyancyOffset) * 100f;

        onBuoyancyLevelChanged?.Invoke(currentBuoyancyLevel);
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

    private float GetPressureAtDepth()
    {
        float depthMeters = Mathf.Abs(transform.position.y - surfaceY);
        return 1f + (depthMeters / 10f); // 1 atm at surface + 1 atm per 10m
    }

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

        float pressureAtDepth = GetPressureAtDepth();
        float buoyancyForce = CalculateBuoyancyForce(pressureAtDepth);
        rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
    }

    private float CalculateBuoyancyForce(float pressureAtDepth)
    {
        if (currentBcdAir <= 0f)
            return 0f;

        // Simulate compressed BCD air, scaled up for gameplay feel
        float effectiveVolume = (currentBcdAir / pressureAtDepth) * buoyancyBoostMultiplier;

        return bcdForce * (effectiveVolume / maxBcdAir);
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
        Vector3 left = new Vector3(-100f, 0f, 0f);
        Vector3 right = new Vector3(100f, 0f, 0f);

        // Surface line
        Gizmos.color = Color.yellow;
        left.y = right.y = surfaceY;
        Gizmos.DrawLine(left, right);

        // Bottom line
        Gizmos.color = Color.red;
        left.y = right.y = bottomY;
        Gizmos.DrawLine(left, right);

        // Neutral buoyancy top
        Gizmos.color = Color.green;
        left.y = right.y = neutralTopY;
        Gizmos.DrawLine(left, right);

        // Neutral buoyancy bottom
        left.y = right.y = neutralBottomY;
        Gizmos.DrawLine(left, right);
    }

#endif
}